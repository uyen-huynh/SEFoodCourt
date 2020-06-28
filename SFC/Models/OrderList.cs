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
        static Dictionary<int, Order> orders;
        static OrderList instanceOfOrderList = new OrderList();       

        private OrderList()
        {
            orders = new Dictionary<int, Order>();
        }

        public static void addOrder(Order order)
        {
            orders[order.orderId] = order;
        }

        public static int getNumsOfOrders()
        {
            return orders.Count();
        }      
    }
}