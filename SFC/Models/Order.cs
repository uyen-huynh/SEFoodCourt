using Antlr.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace SFC.Models
{
    public class Order
    {
        // Attribute
        public Dictionary<int, int> items;
        public List<Food> foods { get; set; } 
        public int id { get; set; }
        public long totalCost = 0;
        public int numberOfCompleted { get; set; }
        public bool paid { get; set; }
        public String time { get; set; }
        public String request { get; set; }
        public int idService { get; set; }
        public String username { get; set; }

        // Method
        public Order()
        {
            id = OrderList.getNewID();
            items = new Dictionary<int, int>();
            foods = new List<Food>();
        }

        public Food getItem(int foodID)
        {
            return Menu.getMenu().foodList[foodID];
        }

        public bool addItemToCart(int foodId, int quantity)
        {
            if (quantity <= 0) return false;
            if (!Menu.getMenu().checkSufficient(foodId, quantity)) return false;

            if (items.ContainsKey(foodId))
            {
                if (!Menu.getMenu().checkSufficient(foodId, quantity + items[foodId])) return false;
                items[foodId] += quantity;
            }
            else
            {
                items.Add(foodId, quantity);
            }
            totalCost += quantity * Menu.getMenu().foodList[foodId].price;
            return true;
        }

        public void removeCartItem(int foodId)
        {
            totalCost -= items[foodId] * Menu.getMenu().foodList[foodId].price;
            items.Remove(foodId);
            return;
        }

        public void changeQuantity(int foodId, int quantity)
        {
            if (quantity < 0) return;
            if (!Menu.getMenu().checkSufficient(foodId, quantity)) return;

            if (items.ContainsKey(foodId))
            {
                totalCost += (quantity - items[foodId]) * Menu.getMenu().foodList[foodId].price;
                items[foodId] = quantity;
            }
            return;
        }

        public long getTotalCost() => totalCost;

        public bool checkComplete() => numberOfCompleted == items.Count();

        public async System.Threading.Tasks.Task makeOrder()
        {
            /*
             * Ham gi do do cua thanh toan
             */
            // Thanh toan xong se chay dong nay

            foreach (var item in items)
            {
                Food food = Menu.getMenu().foodList[item.Key];
                Menu.getMenu().getListFood()[item.Key].quantity -= item.Value;
                _ = DatabaseService.DBUpdate<Food>(Menu.getMenu().foodList[item.Key], "Menu/" + item.Key.ToString());
                if (food.quantity == item.Value)
                {
                    Menu.getMenu().foodList.Remove(item.Key);
                }               
            }
            

            time = DateTime.Now.ToString("h:mm:ss tt");
            await OrderService.Process(id, items, request);
            OrderList.getOrderList().addNewOrder(this);
            return;
        }

        public void clear()
        {
            items.Clear();
            totalCost = 0;
            numberOfCompleted = 0;
            paid = false;
            request = "";
            return;
        }

    }
}
