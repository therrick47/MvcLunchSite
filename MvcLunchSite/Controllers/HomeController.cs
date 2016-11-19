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

        public List<string> getTopScores()
        {
            Dictionary<int, int> topScores = new Dictionary<int, int>();
            foreach(var item in db.Restaurants){
                topScores.Add(item.ID, 0);
            }
            foreach(var user in db.Users)
            {
                if(user.FirstChoice != null && topScores.ContainsKey(user.FirstChoice.GetValueOrDefault()))
                {
                    topScores[user.FirstChoice.GetValueOrDefault()] += 3;
                }
                if (user.SecondChoice != null && topScores.ContainsKey(user.SecondChoice.GetValueOrDefault()))
                {
                    topScores[user.SecondChoice.GetValueOrDefault()] += 2;
                }
                if (user.ThirdChoice != null && topScores.ContainsKey(user.ThirdChoice.GetValueOrDefault()))
                {
                    topScores[user.ThirdChoice.GetValueOrDefault()] += 1;
                }
            }
            topScores = topScores.OrderByDescending(pair => pair.Value).Take(5).ToDictionary(pair => pair.Key, pair => pair.Value);
            List<string> topScoresOutputList = new List<string>();
            foreach (var rest in topScores)
            {
                var query = from item in db.Restaurants
                            where item.ID.Equals(rest.Key)
                            select item;
                var queryItem = query.FirstOrDefault();
                string finalString = queryItem.name + ": " + rest.Value.ToString();
                topScoresOutputList.Add(finalString);
            }
            return topScoresOutputList;
        }

        public ActionResult Index()
        {
            ViewData["TopScoresList"] = getTopScores();
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
                var query = from item in db.Users
                            where item.Id.Contains(userID)
                            select item;
                var rowToChange = query.FirstOrDefault();
                //First Choice in Vote
                int temp = Int32.Parse(t["Choice1"].ToString());
                rowToChange.FirstChoice = temp;

                //Second Choice in Vote
                temp = Int32.Parse(t["Choice2"].ToString());
                rowToChange.SecondChoice = temp;

                //Third Choice in Vote
                temp = Int32.Parse(t["Choice3"].ToString());
                rowToChange.ThirdChoice = temp;
                db.SaveChanges();

                ViewBag.Message = "Vote successfully cast.";
            }

            else
            {
                ViewBag.Message = "Please log in to vote.";
            }
            
            ViewData["TopScoresList"] = getTopScores();
            ViewData["RestaurantList"] = db.Restaurants.ToList();
            ViewData["MenuList"] = db.Menus.ToList();
            ViewData["MenuItemList"] = db.MenuItems.ToList();
            return View("Index");
        }
    }
}