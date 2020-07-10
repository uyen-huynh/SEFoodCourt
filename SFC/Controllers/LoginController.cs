using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SFC.Models;

namespace SFC.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Manager()
        {
            return View();
        }

        public ActionResult Chef()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CheckValidUser(string username, string password)
        {
            string result = "Success";
            CustomerAccount cur = new CustomerAccount();
            try
            {
                cur = AccountService.CheckLogin(username, password);
            }
            catch (Exception e)
            {
                if (e.Message != null)
                    result = e.Message;
            }
            ViewBag.currentAccount = cur;
            return Json(result, JsonRequestBehavior.AllowGet);
        }


    }
}