﻿using MvcLunchSite.Models;
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

        // GET: Orders
        public ActionResult Index()
        {
            ViewData["TopScoresList"] = getTopScores();
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
        /*public ActionResult Print()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Print()*/
    }
}