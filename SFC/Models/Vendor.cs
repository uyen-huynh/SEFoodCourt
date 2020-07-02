using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFC.Models
{
    public class Vendor
    {
        // Attribute
        public String vendorName { get; set; }
        public int vendorId { get; set; }
        public Dictionary<int, FoodTask> taskList { get; set; }

        // Method
        public Vendor() => taskList = new Dictionary<int, FoodTask>();

        public Vendor(string vendorName, int vendorId)
        {
            this.vendorName = vendorName;
            this.vendorId = vendorId;
            this.taskList = new Dictionary<int, FoodTask>();
        }

        public void addTask(FoodTask task) => taskList.Add(task.orderID, task);

        public async System.Threading.Tasks.Task completeTask(int orderId)
        {
            await OrderList.getOrderList().notifyComplete(orderId, taskList[orderId].Count());
            taskList.Remove(orderId);
            return;
        }
    }
}