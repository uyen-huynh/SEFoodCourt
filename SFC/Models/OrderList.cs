using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace SFC.Models
{
    public class OrderList
    {
        // Attribute
        private static OrderList instance;
        public Dictionary<int, Order> orders { get; set; }
        public static int currentID = 0;

        // Method
        private OrderList()
        {
            orders = new Dictionary<int, Order>();
        }

        public static OrderList getOrderList()
        {
            if (instance == null)
            {
                instance = new OrderList();
            }
            return instance;
        }

        public Order getOrder(int orderId)
        {
            if (orders.TryGetValue(orderId, out Order item))
            {
                return item;
            }
            return null;
        }

        public static int getNewID()
        {
            return currentID++;
        }

        public async System.Threading.Tasks.Task notifyComplete(int orderId, int number)
        {
            if (orders.TryGetValue(orderId, out Order order))
            {
                order.numberOfCompleted += number;
                if (order.checkComplete())
                {
                    /*
                     * Method hien thi thong bao
                     */

                    // Store order when it completed
                    await this.storeOrder(orderId);
                }
            }
            return;
        }

        public void addNewOrder(Order order)
        {
            if (order != null)
            {
                orders.Add(order.id, order);
            }
            return;
        }

        public void removeOrder(int orderId)
        {
            orders.Remove(orderId);
            return;
        }

        public async System.Threading.Tasks.Task storeOrder(int orderId)
        {
            try
            {
                await DatabaseService.DBWrite<Order>(orders[orderId], "Order/" + orderId.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Execute fail: " + e.Message.ToString());
            }

            return;
        }
    }
}