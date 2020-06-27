﻿using System;
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
            items.Add(new Food(20, 0, "Salad", "/assets/images/slider-2-696x464-556x371.jpeg", 1,"Food"));
            items.Add(new Food(10, 0, "Cake", "/assets/images/gallery-img-2-506x337-506x337.jpg", 2,"Snack"));
            items.Add(new Food(30, 0, "Beefsteek", "/assets/images/slider-3-696x464.jpg", 3,"Food"));
            items.Add(new Food(35, 0, "Fried rice", "/assets/images/gallery-img-4-696x464.jpg", 4,"Drink"));

            cartItems = new List<Food>();

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

        public static void addCartItem(int id, int quantity)
        {
            for (int i = 0; i < cartItems.Count(); i++)
            {
                if (cartItems[i].id == id)
                {
                    cartItems[i].quantity += quantity;
                    return;
                }
            }

            for (int i = 0; i < items.Count(); i++)
            {
                if (items[i].id == id)
                {
                    cartItems.Add(new Food(items[i].price, items[i].quantity + quantity, items[i].name, items[i].src, id,items[i].type));
                    return;
                }
            }
        }
    }
}