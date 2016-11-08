using MvcLunchSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcLunchSite.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            ViewData["RestaurantList"] = db.Restaurants.ToList();
            ViewData["MenuList"] = db.Menus.ToList();
            ViewData["MenuItemList"] = db.MenuItems.ToList();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
       /* public ActionResult Vote()
        {
            //Console.WriteLine("HEY");
            ViewData["RestaurantList"] = db.Restaurants.ToList();
            ViewData["MenuList"] = db.Menus.ToList();
            ViewData["MenuItemList"] = db.MenuItems.ToList();
            ViewBag.Message = "UPDATDED";
            return View("Index");
        } */

       [HttpPost]
        public ActionResult Vote(FormCollection t)
        {
            string userID = t["userid"].ToString();
            if (userID.Length > 0)
            {
                //First Choice in Vote
                int temp = Int32.Parse(t["Choice1"].ToString());
                VoteRecord firstChoice = new VoteRecord(userID, temp, 3);
                db.VoteRecords.Add(firstChoice);

                //Second Choice in Vote
                temp = Int32.Parse(t["Choice2"].ToString());
                VoteRecord secondChoice = new VoteRecord(userID, temp, 2);
                db.VoteRecords.Add(secondChoice);

                //Third Choice in Vote
                temp = Int32.Parse(t["Choice3"].ToString());
                VoteRecord thirdChoice = new VoteRecord(userID, temp, 1);
                db.VoteRecords.Add(thirdChoice);
                db.SaveChanges();

                ViewBag.Message = "Vote successfully cast.";
            }

            else
            {
                ViewBag.Message = "Please log in to vote.";
            }
            

            ViewData["RestaurantList"] = db.Restaurants.ToList();
            ViewData["MenuList"] = db.Menus.ToList();
            ViewData["MenuItemList"] = db.MenuItems.ToList();
            return View("Index");
        }
    }
}