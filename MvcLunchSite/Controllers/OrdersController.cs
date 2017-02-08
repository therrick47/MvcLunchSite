using MvcLunchSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcLunchSite.Controllers
{
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Orders
        public ActionResult Index()
        {
            ViewData["UserList"] = db.Users.ToList();
            ViewData["OrderList"] = db.Orders.ToList();
            ViewData["RestaurantList"] = db.Restaurants.ToList();
            return View();
        }

        public ActionResult Create()
        {
            ViewBag.orderID = new SelectList(db.Orders, "orderID", "menuItemID", RouteData.Values["id"]);
            return View();
        }
    }
}