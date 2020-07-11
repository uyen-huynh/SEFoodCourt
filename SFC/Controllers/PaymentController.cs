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
                TempData["Order"] = order;
            }
            else
            {
                order = (Order)TempData["Order"];
                TempData.Keep();
            }
            order.totalCost = 30000;
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
            wallet = new MomoWallet(order.totalCost, order.id.ToString());
            wallet.sendPaymentRequest();
            

            JObject jmessage = JObject.Parse(wallet.getJsonResponse());

            string url = jmessage.GetValue("payUrl").ToString();
            string qrcode = jmessage.GetValue("qrCodeUrl").ToString();
            return Json(new { responseUrl = url, qrCode = qrcode});
        }

        void notifyPaymentObservers()
        {
            order.makeOrder();
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
        public JsonResult checkPaid()
        {
            order = (Order)TempData["Order"];       
            if (OrderList.orders.ContainsKey(order.id))
            {
                order.paid = true;
                OrderList.orders[order.id] = order;
                TempData["Order"] = null;
                notifyPaymentObservers();
            }
            TempData.Keep();
            
            return Json(new { isPaid = order.paid});
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


    }
    
    interface Wallet
    {
        void sendPaymentRequest();
        string getJsonResponse();
        void handleIPN(Stream IPNReceiver);
        string getJsonIPN();
    }

    public class MomoWallet : Wallet
    {
        long totalCost;
        string orderInfo;
        string jsonResponse;
        string jsonIPN;

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
            string returnUrl = "http://522a0f73086c.ngrok.io/Payment/HandleResultPayment/";
            string notifyUrl = "http://522a0f73086c.ngrok.io/Payment/HandleIPN";
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

            

     
    }
}