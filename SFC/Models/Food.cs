using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
namespace SFC.Models
{
    public class Food
    {
        // Attribute
        public int id { get; set; }
        [Required(ErrorMessage = "Name required")]
        [DisplayName("Food Name")]
        public string name { get; set; }
        [Required(ErrorMessage = "Price required")]
        [DisplayName("Food Price")]
        public long price { get; set; }
        public int quantity { get; set; }
        [Required(ErrorMessage = "Image Path required")]
        [DisplayName("Food Image")]
        public string imageSrc { get; set; }
        [Required(ErrorMessage = "Vendor ID Required")]
        [DisplayName("Vendor ID")]
        public int vendorIDService { get; set; }
        [DisplayName("Type")]
        public string type { get; set; }
        [DisplayName("Description")]
        public string description { get; set; }

        // Method
        public Food()
        {

        }

        public Food(int id, string name, long price, int quantity, string imageSrc, int vendorIDService, string type, string description)
        {
            this.id = id;
            this.name = name;
            this.price = price;
            this.quantity = quantity;
            this.imageSrc = imageSrc;
            this.vendorIDService = vendorIDService;
            this.type = type;
            this.description = description;
        }

        public Food(Food food)
        {
            this.id = food.id;
            this.name = food.name;
            this.price = food.price;
            this.quantity = food.quantity;
            this.imageSrc = food.imageSrc;
            this.vendorIDService = food.vendorIDService;
            this.type = food.type;
            this.description = food.description;
        }

        public Food Clone()
        {
            return new Food(this);
        }

        public static explicit operator Food(Task<Food> v)
        {
            throw new NotImplementedException();
        }
    }
}