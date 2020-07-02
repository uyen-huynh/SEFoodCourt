using Antlr.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFC.Models
{
    public class Order
    {
        // Attribute
        public Dictionary<int, int> items;
        public int id { get; set; }
        public long totalCost = 0;
        public int numberOfCompleted { get; set; }
        public bool paid { get; set; }
        public String time { get; set; }
        public String request { get; set; }

        // Method
        public Order()
        {
            id = OrderList.getNewID();
            items = new Dictionary<int, int>();
        }

        public Food getItem(int foodID)
        {
            return Menu.getMenu().foodList[foodID];
        }

        public void addItemToCart(int foodId, int quantity)
        {
            if (quantity <= 0) return;
            if (!Menu.getMenu().checkSufficient(foodId, quantity)) return;

            if (items.ContainsKey(foodId))
            {
                items[foodId] += quantity;
            }
            else
            {
                items.Add(foodId, quantity);
            }
            totalCost += quantity * Menu.getMenu().foodList[foodId].price;
            return;
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