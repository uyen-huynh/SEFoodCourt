using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFC.Models
{
    public class FoodList
    {
        public static Dictionary<int, fooditem> foodList;
        static FoodList instance = new FoodList();

        private FoodList()
        {
            DBModel dbModel = new DBModel();
            foodList = dbModel.fooditems.ToDictionary(p => p.id);
        }

        public Dictionary<int, fooditem> getListFood()
        {
            return foodList;
        }

        public static FoodList getInstance()
        {
            return instance;
        }
    }
}