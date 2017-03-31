using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcLunchSite.Models
{
    public class Restaurant
    {
        public int ID { get; set; }
        public string name { get; set; }
        public string pic { get; set; }
        public string url { get; set; }
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