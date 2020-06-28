using Antlr.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFC.Models
{
    public class Order
    {
        public Dictionary<int, int> items;
        public int orderId { get; set; }
        int totalCost = 0;
        bool paid = false;       

        public Order()
        {
            orderId = OrderList.getNumsOfOrders() + 1;
            items = new Dictionary<int, int>();
            OrderList.addOrder(this);
            
        }

        public fooditem getItem(int id)
        {
            return FoodList.foodList[id];
        }

        public Dictionary<int, int> getOrderList()
        {
            return items;
        }

        public int getTotalCost()
        {
            return totalCost;
        }

        public void addItemToCart(int id, int quantity)
        {
            if (items.ContainsKey(id))
            {
                items[id] += quantity;
            }
            else
            {
                items.Add(id, quantity);
            }
            totalCost += quantity * (int)FoodList.foodList[id].price;
        }

        public void changeQuantity(int id, int quantity)
        {
            totalCost += (quantity - items[id]) * (int)FoodList.foodList[id].price;
            items[id] = quantity;
        }

        public void removeCartItem(int id)
        {
            totalCost -= items[id] * (int)FoodList.foodList[id].price;
            items.Remove(id);
        }

    }
}