using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SFC.Models;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Web.Helpers;
using System.Threading;
using Org.BouncyCastle.Asn1.Nist;
using FireSharp.Extensions;
using System.Web.Script.Serialization;
using Microsoft.Ajax.Utilities;
using RestSharp;
using Org.BouncyCastle.Ocsp;
using System.Threading.Tasks;
using ZaloPay.Helper;
using ZaloPay.Helper.Crypto;
using Newtonsoft.Json;

namespace SFC.Controllers
{
    public class PaymentController : Controller
    {
        Order order;
        Wallet wallet;

        public ActionResult Index()
        {
            if (TempData["Order"] == null)
            {
                order = new Order();
                order.id = OrderList.orders.Count();
                TempData["Order"] = order;
            }
            else
            {
                order = (Order)TempData["Order"];
                TempData.Keep();
            }
            return View(order);
        }

        public ActionResult HandleResultPayment(string partnerCode, string accessKey, string requestID, string amount, string orderID, string orderInfo, string orderType, string transID, int errorCode, string mesage, string localMessage, string payType, string responseTime, string extraData, string signature)
        {
            if (errorCode == 0)
            {
                int id = Int32.Parse(orderInfo);
                if (!OrderList.orders.ContainsKey(id))
                {
                    OrderList.orders.Add(id, null);
                }
                TempData["success"] = true;
            }
            else TempData["success"] = false;
            return View();
        }
        
        [HttpPost]
        public JsonResult RequestPayment(string nameWallet)
        {
            order = (Order)TempData["Order"];
            TempData.Keep();
            if (nameWallet == "Momo")
            {
                wallet = new MomoWallet(order.totalCost, order.id.ToString());
            }
            else
            {
                wallet = new ZaloPayWallet(order.totalCost, order.id.ToString());
            }
            wallet.sendPaymentRequest();
            return Json(new { responseUrl = wallet.getPayUrl(), qrCode = wallet.getQRUrl() });
        }

        async System.Threading.Tasks.Task notifyPaymentObservers()
        {
            try
            {
                await DatabaseService.DBWrite<Order>(order, "Order/" + order.id.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Execute fail: " + e.Message.ToString());
            }
            await order.makeOrder();
        }

        [HttpPost]
        public void confirmOrder(string request)
        {
            order = (Order) TempData["Order"];
            TempData.Keep();
            order.request = request;
            TempData["confirmed"] = true;
        }

        [HttpPost]
        public JsonResult CheckPaid()
        {
            order = (Order)TempData["Order"];
            bool paid = false;
            if (order != null && OrderList.orders.ContainsKey(order.id))
            {
                order.paid = paid = true;
                OrderList.orders[order.id] = order;
                TempData.Remove("Order");
                _ = notifyPaymentObservers();
            }
            TempData.Keep();
            
            return Json(new { isPaid = paid});
        }

        
        [HttpPost]
        public void HandleIPN()
        {
            Wallet wallet = new MomoWallet(0, "0");
            using (Stream IPNReceive = Request.InputStream)
            {
                wallet.handleIPN(IPNReceive);
                JObject jmessage = JObject.Parse(wallet.getJsonIPN());

                if (jmessage.GetValue("errorCode").ToString() == "0")
                {
                    int id = Int32.Parse(jmessage.GetValue("orderInfo").ToString());             
                    OrderList.orders.Add(id, null);
                }                
            }
            
        }

        [HttpPost]
        public void ZaloCallBack()
        {
            Wallet wallet = new ZaloPayWallet(0, "0");
            using (Stream IPNReceive = Request.InputStream)
            {
                wallet.handleIPN(IPNReceive);
                JObject jmessage = JObject.Parse(wallet.getJsonIPN());
                
                var ipnJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(wallet.getJsonIPN());


                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(ipnJson["data"].ToString());
                var orderId = Int32.Parse(data["item"].ToString());
                OrderList.orders.Add(orderId, null);
            }

        }


    }
    
    interface Wallet
    {
        void sendPaymentRequest();
        string getJsonResponse();
        void handleIPN(Stream IPNReceiver);
        string getJsonIPN();
        string getPayUrl();
        string getQRUrl();
    }

    public class MomoWallet : Wallet
    {
        long totalCost;
        string orderInfo;
        string jsonResponse;
        string jsonIPN;
        string payUrl;
        string qrUrl;

        public MomoWallet(long totalCost, string orderInfo)
        {
            this.totalCost = totalCost;
            this.orderInfo = orderInfo;

        }

        // Http Request to Momo Server
        public void sendPaymentRequest()
        {
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string orderid = Guid.NewGuid().ToString();
            string requestId = Guid.NewGuid().ToString();
            string partnerCode = "MOMO";
            string accessKey = "F8BBA842ECF85";
            string amount = totalCost.ToString();
            string orderInfo = this.orderInfo;
            string returnUrl = "http://7b9f76d3bd99.ngrok.io/Payment/HandleResultPayment/";
            string notifyUrl = " http://7b9f76d3bd99.ngrok.io/Payment/HandleIPN";
            string secretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";
            string extraData = "email=uyenhuynh@gmail.com";

            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyUrl + "&extraData=" +
                extraData;

            byte[] keyByte = Encoding.UTF8.GetBytes(secretKey);
            byte[] messsageBytes = Encoding.UTF8.GetBytes(rawHash);
            var hmacsha256 = new HMACSHA256(keyByte);
            byte[] hashmessage = hmacsha256.ComputeHash(messsageBytes);
            string hex = BitConverter.ToString(hashmessage);
            string signature = hex.Replace("-", "").ToLower();

            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyUrl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }
            };

            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(endpoint);

                var postData = message.ToString();

                var data = Encoding.UTF8.GetBytes(postData);

                httpWReq.ProtocolVersion = HttpVersion.Version11;
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/json";

                httpWReq.ContentLength = data.Length;
                httpWReq.ReadWriteTimeout = 30000;
                httpWReq.Timeout = 15000;
                Stream stream = httpWReq.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();

                using (var reader = new StreamReader(response.GetResponseStream()))
                {

                    string temp = null;
                    while ((temp = reader.ReadLine()) != null)
                    {
                        jsonResponse += temp;
                    }
                    JObject jmessage = JObject.Parse(jsonResponse);

                    qrUrl = payUrl = jmessage.GetValue("payUrl").ToString();                 
                }
            }
            catch (WebException e)
            {
                // 
            }


        }

        public string getJsonResponse()
        {
            return jsonResponse;
        }

        // Receive IPN from Momo Server
        public void handleIPN(Stream IPNReceive)
        {
            using (StreamReader reader = new StreamReader(IPNReceive, Encoding.ASCII))
            {
                string temp = null;
                while ((temp = reader.ReadLine()) != null)
                {
                    jsonIPN += temp;
                }
            }
            var dict = HttpUtility.ParseQueryString(jsonIPN);
            jsonIPN = new JavaScriptSerializer().Serialize(
                                dict.AllKeys.ToDictionary(k => k, k => dict[k])
                       );
        }

        public string getJsonIPN()
        {
            return jsonIPN;
        }

        public string getPayUrl()
        {
            return payUrl;
        }

        public string getQRUrl()
        {
            return qrUrl;
        }


    }

    public class ZaloPayWallet : Wallet
    {
        long totalCost;
        string orderInfo;
        string jsonResponse;
        string jsonIPN;
        string payUrl;
        string qrUrl;
        static string appid = "12470";
        static string key1 = "QayexiqlF2d2ch91P2oxs9XugMW8F6wQ";

        public ZaloPayWallet(long totalCost, string orderInfo)
        {
            this.totalCost = totalCost;
            this.orderInfo = orderInfo;

        }

        // Http Request to Momo Server
        public void sendPaymentRequest()
        {
            var reqtime = Utils.GetTimeStamp().ToString();
            var transid = Guid.NewGuid().ToString();
            var embeddata = new {};
            var order = new Dictionary<string, string>();

            order.Add("appid", appid);
            order.Add("appuser", "demo");
            order.Add("apptime", Utils.GetTimeStamp().ToString());
            order.Add("amount", totalCost.ToString());
            order.Add("apptransid", DateTime.Now.ToString("yyMMdd") + "_" + transid);
            order.Add("embeddata", JsonConvert.SerializeObject(embeddata));
            order.Add("item", orderInfo);
            order.Add("description", "Smart Food Court ZaloPay demo");
            order.Add("bankcode", "zalopayapp");

            var data = appid + "|" + order["apptransid"] + "|" + order["appuser"] + "|" + order["amount"] + "|"
                + order["apptime"] + "|" + order["embeddata"] + "|" + order["item"];
            order.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

            var orderJSON = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(order));

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("appid", appid);
            param.Add("reqtime", reqtime);
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, appid + "|" + reqtime));

            payUrl = qrUrl = "https://sbgateway.zalopay.vn/openinapp?order=" + Convert.ToBase64String(orderJSON);
        }

        public string getJsonResponse()
        {
            return jsonResponse;
        }

        // Receive IPN from ZaloPay Server
        public void handleIPN(Stream IPNReceive)
        {
            using (StreamReader reader = new StreamReader(IPNReceive, Encoding.ASCII))
            {
                string temp = null;
                while ((temp = reader.ReadLine()) != null)
                {
                    jsonIPN += temp;
                }
            }
            // var dataJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonIPN);
        }

        public string getJsonIPN()
        {
            return jsonIPN;
        }

        public string getPayUrl()
        {
            return payUrl;
        }

        public string getQRUrl()
        {
            return qrUrl;
        }
    }
}