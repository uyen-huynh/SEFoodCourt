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
        
        [HttpGet]
        public ActionResult Index()
        {

            FoodList foodList = FoodList.getObj();
            return View(foodList);
        }

        [HttpPost]
        public ActionResult AddItemToCart()
        {

            FoodList.cartItems.Add(FoodList.cartItems[0]);
            return null;
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