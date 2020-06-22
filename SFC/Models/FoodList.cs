using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFC.Models
{
    public class FoodList
    {
        private static FoodList instance = new FoodList();
        public static List<Food> cartItems { get; set; }
        public static List<Food> items { get; set; }
        private FoodList()
        {
            items = new List<Food>();
            items.Add(new Food(20, 1, "Salad", "/assets/images/slider-2-696x464-556x371.jpeg", 1));
            items.Add(new Food(10, 1, "Cake", "/assets/images/gallery-img-2-506x337-506x337.jpg", 2));
            items.Add(new Food(30, 1, "Beefsteek", "/assets/images/slider-3-696x464.jpg", 3));
            items.Add(new Food(35, 1, "Fried rice", "/assets/images/gallery-img-4-696x464.jpg", 4));

            cartItems = new List<Food>();
            cartItems.Add(new Food(20, 1, "Salad", "/assets/images/slider-2-696x464-556x371.jpeg", 1));
            cartItems.Add(new Food(10, 1, "Cake", "/assets/images/gallery-img-2-506x337-506x337.jpg", 2));

        }

        public List<Food> getCartItems()
        {
            return cartItems;
        }

        public List<Food> getListFood()
        {
            return items;
        }
        public static FoodList getObj()
        {
            return instance;
        }

        public int getTotalCost()
        {
            int sum = 0;
            foreach (var item in cartItems)
            {
                sum += item.quantity * item.price;
            }
            return sum;
        }
    }
}