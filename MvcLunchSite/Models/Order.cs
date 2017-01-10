using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcLunchSite.Models
{
    public class Order
    {
        public int orderID { get; set; }
        public string userID { get; set; }
        public int menuItemID { get; set; }
        public string ItemPrice { get; set; }
        public string menuItemName { get; set; }
        public string menuItemDescription { get; set; }
    }
}