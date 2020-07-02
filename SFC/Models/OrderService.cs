using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFC.Models
{
    public class OrderService
    {
        // Attribute
        private static Menu menu = Menu.getMenu();
        private static VendorList vendorList = VendorList.getVendorList();

        // Method
        public OrderService()
        {
        }

        // Clasify order and send to suitable vendor
        public static async System.Threading.Tasks.Task Process(int orderId, Dictionary<int, int> items, String request)
        {
            Dictionary<int, FoodTask> classification = new Dictionary<int, FoodTask>();

            foreach (var item in items)
            {
                int vendorId = menu.getFood(item.Key).vendorIDService;
                if (!classification.ContainsKey(vendorId))
                {
                    FoodTask task = new FoodTask(orderId, request);
                    task.addTask(item.Key, item.Value);
                    classification.Add(vendorId, task);
                }
                else
                {
                    classification[vendorId].addTask(item.Key, item.Value);
                }

                // Update Menu
                await menu.changeQuantity(item.Key, menu.getFood(item.Key).quantity - item.Value);
            }

            foreach (var item in classification)
            {
                vendorList.transmitTask(item.Key, item.Value);
            }
        }

        // Transfer from Dictionary<int, int> to List<Food>
        public static List<Food> TransferDictToList(Dictionary<int, int> items)
        {
            List<Food> list = new List<Food>();
            foreach (var item in items)
            {
                Food temp = menu.getFood(item.Key);
                temp.quantity = item.Value;
                list.Add(temp);
            }
            return list;
        }
    }
}