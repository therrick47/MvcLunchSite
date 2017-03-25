using MvcLunchSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace MvcLunchSite.Helpers
{
    public class SecurityHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        public bool atLeastSuperuser(IPrincipal user)
        {
            bool allowed = false;
            if ((user != null) && user.Identity.IsAuthenticated)
            {
                var identity_query = from item in db.Users
                                     where item.Role.Equals("superuser") && item.UserName.Equals(user.Identity.Name)
                                     select item;
                var test = identity_query.FirstOrDefault();
                if (test != null)
                {
                    allowed = true;
                }
            }
            return allowed;
        }

        public bool atLeastAdmin(IPrincipal user)
        {
            bool allowed = false;
            if ((user != null) && user.Identity.IsAuthenticated)
            {
                var identity_query = from item in db.Users
                                     where (item.Role.Equals("superuser") || item.Role.Equals("admin")) && item.UserName.Equals(user.Identity.Name)
                                     select item;
                var test = identity_query.FirstOrDefault();
                if (test != null)
                {
                    allowed = true;
                }
            }
            return allowed;
        }

        public bool atLeastOrderer(IPrincipal user)
        {
            bool allowed = false;
            if ((user != null) && user.Identity.IsAuthenticated)
            {
                var identity_query = from item in db.Users
                                     where (item.Role.Equals("superuser") || item.Role.Equals("admin") || item.Role.Equals("orderer")) && item.UserName.Equals(user.Identity.Name)
                                     select item;
                var test = identity_query.FirstOrDefault();
                if (test != null)
                {
                    allowed = true;
                }
            }
            return allowed;
        }

    }
}