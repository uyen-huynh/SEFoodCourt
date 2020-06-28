using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SFC.Models;
using System.IO;
using System.Data.Entity.Infrastructure;

//using MySql.Data.MySqlClient;
namespace SFC.Controllers
{
    public class ManagerController : Controller
    {
        //Food _food;
        //public ManagerController(Food food)
        //{
        //    _food = food;
        //}
        // GET: Manager                
        public ActionResult HomeManager() {
            
            List<fooditem> listFood = new List<fooditem>();
            using (DBModel dbModel = new DBModel())
            {
                listFood = dbModel.fooditems.ToList<fooditem>();
            }           
            return View(listFood);
        }
        [HttpGet]
        public ActionResult Form()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Form(fooditem food)
        {
            if (ModelState.IsValid)
            {
                using(DBModel dbModel = new DBModel())
                {
                    dbModel.fooditems.Add(food);
                    dbModel.SaveChanges();
                }
                return RedirectToAction("HomeManager");
            }
            return RedirectToAction("HomeManager");
            //return View(food);            
        }
    }
}