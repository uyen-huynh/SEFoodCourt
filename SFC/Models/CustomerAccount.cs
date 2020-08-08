using System;
using System.Dynamic;
using System.Linq;

namespace SFC.Models
{
    public class CustomerAccount
    {
        public string username { get; set; }
        public string cipherPassword { get; set; }
        public string name { get; set; }
        public string birthYear { get; set; }
        public string email { get; set; }

        public CustomerAccount() {
            this.username = "Login";
        }
        public CustomerAccount(string username, string plainPassword, string name, string birthYear, string email)
        {
            this.username = username;
            cipherPassword = AccountService.ComputeMD5Hash(plainPassword);
            this.name = name;
            this.birthYear = birthYear;
            this.email = email;
        }
        public CustomerAccount(CustomerAccount src)
        {
            username = src.username;
            cipherPassword = src.cipherPassword;
            name = src.name;
            birthYear = src.birthYear;
            email = src.email;
        }

        public int getPoint()
        {
            if (OrderList.orders.Count() == 0) return 1;
            return 1 + (int) OrderList.orders.Values.Last().totalCost / 10000;
        }

        public int getOrderId()
        {
            if (OrderList.orders.Count() == 0) return -1;
            return OrderList.orders.Keys.Last();
        }
    }
}
