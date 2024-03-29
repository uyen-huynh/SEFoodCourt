﻿using SFC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFC.Controllers
{
    public class ManagerController : Controller
    {
        // GET: Manager

        public ActionResult OrderTable()
        {
            if (Session["vendorID"] == null)
            {
                return RedirectToAction("Index", "Manager");
            }
            int id = (int)Session["vendorID"];
            ViewData["vendorID"] = id;
            return View();
        }
        public ActionResult OrderData()
        {
            List<Order> orders = new List<Order>();
            try
            {
                orders = DatabaseService.DBGetList<Order>("Order/");
                orders.RemoveAll(x => x == null);
            }
            catch (Exception e)
            {
                Console.WriteLine("Execute fail: " + e.Message.ToString());
            }
            //return View(listFood);                        
            return Json(new { data = orders }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MenuTable()
        {
            if (Session["vendorID"] == null)
            {
                return RedirectToAction("Index", "Manager");
            }
            int id = (int)Session["vendorID"];
            ViewData["vendorID"] = id;
            return View();
        }
        public ActionResult MenuData()
        {          
            List<Food> menu = new List<Food>();
            try
            {
                menu = DatabaseService.DBGetList<Food>("Menu/");
                menu.RemoveAll(x => x == null);
            }
            catch (Exception e)
            {
                Console.WriteLine("Execute fail: " + e.Message.ToString());
            }
            //return View(listFood);                        
            return Json(new { data = menu }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult AddOrUpdate(int id = 0)
        {            
            List<Food> menu = new List<Food>();
            try
            {
                menu = DatabaseService.DBGetList<Food>("Menu/");
                menu.RemoveAll(x => x == null);
            }
            catch (Exception e)
            {
                Console.WriteLine("Execute fail: " + e.Message.ToString());
            }
            Food f = null;
            for (int i = 0; i < menu.Count; ++i)
            {
                Food temp_food = menu.ElementAt(i);
                if (temp_food.id == id)
                {
                    f = temp_food;
                }
            }
            if (f == null)
            {
                return View(new Food());
            }
            else return View(f);

        }
        [HttpPost]
        public async Task<ActionResult> AddOrUpdate(Food food)
        {            
            Menu list = Menu.getMenu();            
            if (food.id == 0)
            {                              
                await list.addFood(food);
                //await DatabaseService.DBWrite<Food>(food, "Menu/" + food.id.ToString());
                //await menu1.addFood(food);
                return Json(new { success = true, message = "Saved successfully" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //await DatabaseService.DBUpdate<Food>(food, "Menu/" + food.id.ToString());
                await list.update(food.id, food);
                return Json(new { success = true, message = "Updated successfully" }, JsonRequestBehavior.AllowGet);
            }            
        }
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {            
            Menu list = Menu.getMenu();           
            await list.removeFood(id);
            return Json(new { success = true, message = "Deleted successfully" }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult Report()
        {
            if (Session["vendorID"] == null)
            {
                return RedirectToAction("Index", "Manager");
            }
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CheckValidUser(string username, string password)
        {
            string result = "Success";
            ManagerAccount cur = new ManagerAccount();
            try
            {
                cur = ManagerAccountService.CheckLogin(username, password);
            }
            catch (Exception e)
            {
                if (e.Message != null)
                    result = e.Message;
            }
            ViewBag.currentAccount = cur;
            Session["vendorID"] = cur.id;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DashBoard()
        {
            return View();
        }

        public ActionResult Index()
        {           
            return View();
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();            
            return RedirectToAction("Index", "Manager");
        }

        public ActionResult ProfileVendorOwner()
        {
            if (Session["vendorID"] == null)
            {
                return RedirectToAction("Index", "Manager");
            }

            int id = (int)Session["vendorID"];
            

            List<ManagerAccount> vendors = new List<ManagerAccount>();
            try
            {
                vendors = DatabaseService.DBGetList<ManagerAccount>("Account/VendorOwner");
                vendors.RemoveAll(x => x == null);
            }
            catch (Exception e)
            {
                Console.WriteLine("Execute fail: " + e.Message.ToString());
            }
            ManagerAccount f = null;
            for (int i = 0; i < vendors.Count; ++i)
            {
                ManagerAccount account = vendors.ElementAt(i);
                if (account.id == id)
                {
                    f = account;
                }
            }
            ViewData["vendor"] = f;
            return View();
        }
    }
}