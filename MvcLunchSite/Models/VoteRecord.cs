using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcLunchSite.Models
{
    public class VoteRecord
    {
        public int Id { get; set; }
        public string UserID { get; set; }
        public int Value { get; set; }
        public int RestaurantID { get; set; }

        public VoteRecord(string UserID, int RestaurantID, int Value)
        {
            this.UserID = UserID;
            this.RestaurantID = RestaurantID;
            this.Value = Value;
        }
    }
}