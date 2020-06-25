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
        public void AddItemToCart(int id, int quantity)
        {
            FoodList.addCartItem(id, quantity);
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