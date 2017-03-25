using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcLunchSite.Models
{
    public class Manage
    {
        
        public int Id { get; set; }
        public DateTime voteEndDate { get; set; }
        public DateTime orderEndDate { get; set; }
    }
}