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
            if (order == null)
            {
                if (TempData["Order"] == null)
                {
                    order = new Order();
                    TempData["Order"] = order;
                } 
                else {
                    order = (Order) TempData["Order"];
                }              
            }

            FoodList foodList = FoodList.getInstance();
            TempData.Keep();
            return View(foodList);
        }

        [HttpPost]
        public void AddItemToCart(int id, int quantity)
        {
            Order order = (Order)TempData["Order"];
            TempData.Keep();
            order.addItemToCart(id, quantity);
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

    }
}