using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperLunchDecider.Models
{
    public class Restaurant
    {
        public int restaurantID { get; set; }
        public string restaurantName { get; set; }
        public ICollection<Menu> Menus { get; set; }
        public List<Restaurant> GetRestaurants()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<Restaurant> restaurants = new List<Restaurant>();
            foreach (Restaurant r in db.Restaurants)
                restaurants.Add(r);
            return restaurants;
        }
    }
}