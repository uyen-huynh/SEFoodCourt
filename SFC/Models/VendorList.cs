using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace SFC.Models
{
    public class VendorList
    {
        // Attribute
        private static VendorList instance;
        public Dictionary<int, Vendor> vendorList;

        // Method
        private VendorList()
        {
            vendorList = new Dictionary<int, Vendor>();
        }

        public static VendorList getVendorList()
        {
            if (instance == null)
            {
                instance = new VendorList();
            }
            return instance;
        }

        public void addVendor(Vendor vendor)
        {
            if (!vendorList.ContainsKey(vendor.vendorId))
            {
                vendorList.Add(vendor.vendorId, vendor);
            }
            return;
        }

        public void removeVendor(int vendorId)
        {
            vendorList.Remove(vendorId);
            return;
        }

        public void transmitTask(int vendorId, FoodTask task)
        {
            if (vendorList.ContainsKey(vendorId))
            {
                vendorList[vendorId].addTask(task);
            }
            return;
        }
    }
}