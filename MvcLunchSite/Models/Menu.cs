using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcLunchSite.Models
{
    public class Menu
    {
        public int menuID { get; set; }
        public int restaurantID { get; set; }
        public string menuName { get; set; }
        public Restaurant Restaurant { get; set; }
        public ICollection<MenuItem> MenuItems { get; set; }
    }
}