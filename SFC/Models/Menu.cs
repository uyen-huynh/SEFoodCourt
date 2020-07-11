using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFC.Models
{
    public class Menu
    {
        private static Menu instance;
        public Dictionary<int, Food> foodList { get; set; }
        public static int maxId = 0;
        // Method
        private Menu()
        {
            // Get menu from database
            foodList = new Dictionary<int, Food>();

            List<Food> res = new List<Food>();
            try
            {
                res = DatabaseService.DBGetList<Food>("Menu/");
            }
            catch (Exception e)
            {
                Console.WriteLine("Execute fail: " + e.Message.ToString());
            }
            if (res != null)
            {
                foreach (var food in res)
                {
                    foodList.Add(food.id, food);
                    maxId = (food.id > maxId) ? food.id : maxId;
                }
            }
        }

        public Dictionary<int, Food> getListFood() => foodList;

        public int Count() => foodList.Count();

        public static Menu getMenu()
        {
            if (instance == null)
            {
                instance = new Menu();
            }
            return instance;
        }

        public Food getFood(int foodId)
        {
            if (foodList.TryGetValue(foodId, out Food item))
            {
                return item.Clone();
            }
            return null;
        }

        public bool checkSufficient(int foodId, int number)
        {
            if (foodList.TryGetValue(foodId, out Food item))
            {
                return item.quantity >= number;
            }
            return false;
        }

        public List<Food> sortedMenu()
        {
            List<Food> allItems = foodList.Values.ToList();
            allItems.Sort((x, y) => x.price.CompareTo(y.price));
            return allItems;
        }

        public async System.Threading.Tasks.Task addFood(Food food)
        {
            if (food.id != 0)
            {
                await this.changeQuantity(food.id, foodList[food.id].quantity + food.quantity);
            }
            else
            {
                food.id = ++maxId;
                // Update Database
                try
                {
                    await DatabaseService.DBWrite<Food>(food, "Menu/" + food.id.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Execute fail: " + e.Message.ToString());
                }

                foodList.Add(food.id, food);
            }
            return;
        }

        public async System.Threading.Tasks.Task removeFood(int foodId)
        {
            // Update Database
            try
            {
                await DatabaseService.DBDelete("Menu/" + foodId.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Execute fail: " + e.Message.ToString());
            }

            foodList.Remove(foodId);
            return;
        }

        public async System.Threading.Tasks.Task changeQuantity(int foodId, int number)
        {
            if (number < 0) return;
            if (foodList.ContainsKey(foodId))
            {
                Food temp = this.getFood(foodId);
                temp.quantity = number;
                await this.update(foodId, temp);
            }
            return;
        }

        public async System.Threading.Tasks.Task update(int foodId, Food food)
        {
            if (foodList.ContainsKey(foodId))
            {
                // Update Database
                try
                {
                    await DatabaseService.DBUpdate<Food>(food, "Menu/" + foodId.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Execute fail: " + e.Message.ToString());
                }

                foodList[foodId] = food.Clone();
            }
            return;
        }
    }
}