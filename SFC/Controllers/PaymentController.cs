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

namespace SFC.Controllers
{
    public class PaymentController : Controller
    {
        Order order;

        public ActionResult Index()
        {
            order = (Order)TempData["Order"];
            TempData.Keep();
            return View(order);
        }

        public ActionResult HandleResultPayment(string partnerCode, string accessKey, string requestID, string amount, string orderID, string orderInfo, string orderType, string transID, int errorCode, string mesage, string localMessage, string payType, string responseTime, string extraData, string signature)
        {
            if (TempData["success"] == null)
                TempData.Add("success", true);
            if (errorCode != 0)
            {
                TempData["success"] = false;
                TempData.Keep();
            }
            return View();
        }

        public static string sendPaymentRequest(string endpoint, string postJsonString)
        {

            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(endpoint);

                var postData = postJsonString;

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

                string jsonresponse = "";

                using (var reader = new StreamReader(response.GetResponseStream()))
                {

                    string temp = null;
                    while ((temp = reader.ReadLine()) != null)
                    {
                        jsonresponse += temp;
                    }
                }


                //todo parse it
                return jsonresponse;
                //return new MomoResponse(mtid, jsonresponse);

            }
            catch (WebException e)
            {
                return e.Message;
            }
        }

        [HttpPost]
        public JsonResult SendRequest()
        {
            order = (Order)TempData["Order"];
            TempData.Keep();

            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string orderid = Guid.NewGuid().ToString();
            string requestId = Guid.NewGuid().ToString();
            string partnerCode = "MOMO";
            string accessKey = "F8BBA842ECF85";
            string amount = (order.getTotalCost()).ToString();
            string orderInfo = "order info";
            string returnUrl = "http://75a249bf72c8.ngrok.io/Payment/HandleResultPayment/";
            string notifyUrl = "https://momo.vn";
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
            string responseFromMomo = sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            string url = jmessage.GetValue("payUrl").ToString();
            string qrcode = jmessage.GetValue("qrCodeUrl").ToString();
            return Json(new { responseUrl = url, qrCode = qrcode});
        }


    }
}