using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Data.Entity;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFC.Models;

namespace SFC.Controllers
{
    public class ChefController : Controller
    {
        // GET: Chef
        public List<Food> getListFoods()
        {
            List<Food> list = new List<Food>();
            list = DatabaseService.DBGetList<Food>("Menu");
            return list;
        }
        public List<Order> getListOrder()
        {
            List<Order> list = new List<Order>();
            list = DatabaseService.DBGetList<Order>("Order");
            return list;
        }
        public async Task<Order> getOrderWithNameFood(int id)
        {
            Order order = await DatabaseService.DBGetRecord<Order>("Order/" + id.ToString());
            List<Food> listFood = DatabaseService.DBGetList<Food>("Menu");
            List<string> key = new List<string>(order.items.Keys);
            for (int i = 0; i < key.Count(); i++)
            {
                for (int j = 0; j < listFood.Count(); j++)
                {
                    if (listFood[j] != null)
                    {
                        if (listFood[j].id == int.Parse(key[i].Replace("\"", "")))
                        {
                            order.foods.Add(new Food() { id = listFood[j].id, name = listFood[j].name, quantity = order.items[key[i]] });
                        }
                    }
                    else { continue; }
                }
            }
            return order;
        }
        public ActionResult Index()
        {
            List<Order> list = getListOrder();
            List<Food> listFood = getListFoods();
            List<Order> orders = new List<Order>();
            for (int i = 0; i < list.Count(); i++)
            {
                List<string> key = new List<string>(list[i].items.Keys);
                for (int j = 0; j < key.Count(); j++)
                {
                    for (int k = 0; k < listFood.Count(); k++)
                    {
                        if (listFood[k] != null)
                        {
                            if (listFood[k].id == int.Parse(key[j].Replace("\"", "")))
                            {
                                list[i].foods.Add(new Food() { id = listFood[k].id, name = listFood[k].name, quantity = list[i].items[key[j]] });
                            }
                        }
                        else { continue; }
                    }
                }
            }
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].numberOfCompleted == 0)
                {
                    orders.Add(new Order() { id = list[i].id, numberOfCompleted = list[i].numberOfCompleted, username = list[i].username, foods = list[i].foods, items = list[i].items });
                }
            }

            return View(orders);
        }
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            Order order = await getOrderWithNameFood(id);
            return View(order);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Order order)
        {
            Order order1 = await DatabaseService.DBGetRecord<Order>("Order/" + order.id.ToString());
            order1.numberOfCompleted = order1.items.Count();
            await DatabaseService.DBUpdate<Order>(order1, "Order/" + order.id.ToString());
            return RedirectToAction("Index");
        }

        public ActionResult Complete()
        {
            List<Order> list = getListOrder();
            List<Food> listFood = getListFoods();
            List<Order> orders = new List<Order>();
            for (int i = 0; i < list.Count(); i++)
            {
                List<string> key = new List<string>(list[i].items.Keys);
                for (int j = 0; j < key.Count(); j++)
                {
                    for (int k = 0; k < listFood.Count(); k++)
                    {
                        if (listFood[k] != null)
                        {
                            if (listFood[k].id == int.Parse(key[j].Replace("\"", "")))
                            {
                                list[i].foods.Add(new Food() { id = listFood[k].id, name = listFood[k].name, quantity = list[i].items[key[j]] });
                            }
                        }
                        else { continue; }
                    }
                }
            }
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].numberOfCompleted != 0)
                {
                    orders.Add(new Order() { id = list[i].id, numberOfCompleted = list[i].numberOfCompleted, username = list[i].username, foods = list[i].foods, items = list[i].items });
                }
            }

            return View(orders);
        }

        public async Task<ActionResult> sendNotification(String name, int id)
        {
            CustomerAccount acc = await DatabaseService.DBGetRecord<CustomerAccount>("Account/Customer/" + name);
            Order order = await getOrderWithNameFood(id);
            var senderEmail = new MailAddress(ManagerAccountService.current.email, ManagerAccountService.current.name);
            var receiverEmail = new MailAddress(acc.email);
            var password = "vendor123";
            var sub = "Notice of completed order";
            var body = "Dear ";
            body = body + acc.name + ",\n\nYour order includes:\n";

            for (int i = 0; i < order.foods.Count; i++)
            {
                body = body + order.foods[i].name + "  x" + order.foods[i].quantity.ToString() + "\n";
            }
            body = body + "\ncompleted, please go to " + ManagerAccountService.current.name + " to receive food\n" +
                          "\nThank you and see you again !";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail.Address, password)
            };
            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = sub,
                Body = body
            })
            {
                smtp.Send(mess);
            }
            return RedirectToAction("Complete");
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
    }
}
