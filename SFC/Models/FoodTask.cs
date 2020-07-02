using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFC.Models
{
    public class FoodTask
    {
        // Attribute
        public Dictionary<int, int> tasks { get; set; }
        public int orderID { get; set; }
        public String request { get; set; }

        // Method
        public FoodTask()
        {
            tasks = new Dictionary<int, int>();
        }

        public FoodTask(int orderID, string request)
        {
            tasks = new Dictionary<int, int>();
            this.orderID = orderID;
            this.request = request;
        }

        public int Count()
        {
            return tasks.Count();
        }

        public void addTask(int foodId, int number)
        {
            tasks.Add(foodId, number);
            return;
        }
    }
}