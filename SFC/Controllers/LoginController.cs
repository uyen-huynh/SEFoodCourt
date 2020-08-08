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
            if (AccountService.current.username == "Login")
                return View();
            return RedirectToAction("Account");
        }

        public ActionResult Account()
        {
            return View(AccountService.current);
        }

        public ActionResult Signup()
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

        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> SignUp(string username, string password, string name, string birthYear, string email)
        {
            string result = "Success";
            try
            {
                bool check = await AccountService.CheckSignUp(username, password, name, birthYear, email);
            }
            catch (Exception e)
            {
                if (e.Message != null)
                    result = e.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}