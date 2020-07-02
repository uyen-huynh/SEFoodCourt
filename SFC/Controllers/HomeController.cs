using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SFC.Models;

namespace SFC.Controllers
{
    public class HomeController : Controller
    {
        Order order;

        [HttpGet]
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

            Menu foodList = Menu.getMenu();
            
            return View(foodList);
        }

        public ActionResult AboutUs()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        /* ----------------------------------- METHODS FOR CART---------------------*/
        public ActionResult Cart()
        {
            Order order = (Order)TempData["Order"];
            TempData.Keep();

            return View(order);
        }

        [HttpPost]
        public void AddItemToCart(int id, int quantity)
        {
            Order order = (Order)TempData["Order"];
            TempData.Keep();
            order.addItemToCart(id, quantity);
        }

        [HttpPost]
        public JsonResult ChangeQuantity(int id, int quantity)
        {
            //
            order = (Order)TempData["Order"];
            TempData.Keep();

            order.changeQuantity(id, quantity);
            return Json(new { totalCost = order.getTotalCost() });
        }

        public JsonResult RemoveItem(int id)
        {
            //
            order = (Order)TempData["Order"];
            TempData.Keep();

            order.removeCartItem(id);
            return Json(new { totalCost = order.getTotalCost() });
        }
    }
}