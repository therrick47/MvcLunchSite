using MvcLunchSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using MvcLunchSite.Helpers;

namespace MvcLunchSite.Controllers
{
    public class OrdersController : Controller
    {
        private SecurityHelper sh = new SecurityHelper();
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<string> getTopScores()
        {
            Dictionary<int, int> topScores = new Dictionary<int, int>();
            foreach (var item in db.Restaurants)
            {
                topScores.Add(item.ID, 0);
            }
            foreach (var user in db.Users)
            {
                if (user.FirstChoice != null && topScores.ContainsKey(user.FirstChoice.GetValueOrDefault()))
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

        public int getDropDownIndexForWinningRestaurant()
        {
            Dictionary<int, int> topScores = new Dictionary<int, int>();
            foreach (var item in db.Restaurants)
            {
                topScores.Add(item.ID, 0);
            }
            foreach (var user in db.Users)
            {
                if (user.FirstChoice != null && topScores.ContainsKey(user.FirstChoice.GetValueOrDefault()))
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
            Tuple<int, int> winner = new Tuple<int, int>(0, 0);
            foreach (var element in topScores)
            {
                if (element.Value > winner.Item2)
                {
                    winner = new Tuple<int, int>(element.Key, element.Value);
                }
            }
            return winner.Item1;
        }

        // GET: Orders
        public ActionResult Index()
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastOrderer(user))
            {
                ViewData["TopScoresList"] = getTopScores();
                ViewData["UserList"] = db.Users.ToList();
                ViewData["OrderList"] = db.Orders.ToList();
                ViewData["RestaurantList"] = db.Restaurants.ToList();
                ViewData["WinningRestaurantDropDownIndex"] = getDropDownIndexForWinningRestaurant();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Create()
        {
            //Commenting this code out for the time being because I don't know if it is being used at all
            //ViewBag.orderID = new SelectList(db.Orders, "orderID", "menuItemID", RouteData.Values["id"]);
            //return View();
            return RedirectToAction("Index", "Home");
        }
    }
}