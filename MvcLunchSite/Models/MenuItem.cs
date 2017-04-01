using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcLunchSite.Models
{
    public class MenuItem
    {
        public int menuItemID { get; set; }
        public string menuItemName { get; set; }
        public string menuItemPrice { get; set; }
        public string menuItemDescription { get; set; }
        public int menuID { get; set; }
        public Menu Menu { get; set; }
        public string itemType { get; set; }
    }
}