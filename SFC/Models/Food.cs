using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFC.Models
{
    public class Food
    {
        public int price { get; set; }
        public int quantity { get; set; }
        public String name { get; set; }
        public string src { get; set; }
        public int id { get; }
        public String type { get; set; }

        public Food(int price, int quantity, String name, String src, int id, String type)
        {
            this.price = price;
            this.quantity = quantity;
            this.name = name;
            this.src = src;
            this.id = id;
            this.type = type;
        }
    }
}