using MvcLunchSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;

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
            ViewData["OrderList"] = db.Orders.ToList();
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

        /*public ActionResult OrderItem()
        {
            ViewBag.orderID = new SelectList(db.Orders, ""
        }*/

        //public ActionResult OrderItem()
        //{
        //    ViewBag.orderID = new SelectList(db.Orders, "orderID", "userID", "menuItemID", RouteData.Values["id"]);

        //    return View();
        //}

        [HttpPost]
        public JsonResult OrderItem([Bind(Include = "menuItemID,customization")] Order order)
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if ((user != null) && user.Identity.IsAuthenticated)
            {
                string userId = user.Identity.GetUserId();
                if(order.menuItemID != 0)
                {
                    var query = from item in db.MenuItems
                                where item.menuItemID.Equals(order.menuItemID)
                                select item;
                    MenuItem menuItem = query.FirstOrDefault();
                    if(menuItem != null)
                    {
                        order.menuItemDescription = menuItem.menuItemDescription;
                        order.ItemPrice = menuItem.menuItemPrice;
                        order.menuItemName = menuItem.menuItemName;
                        order.userID = userId;
                        var secondQuery = from item in db.Menus
                                          where item.menuID.Equals(menuItem.menuID)
                                          select item;
                        Menu menu = secondQuery.FirstOrDefault();
                        if(menu == null)
                        {
                            return Json(new {error = "Restaurant was not found." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            order.restaurantID = menu.restaurantID.ToString();
                        }
                        if (ModelState.IsValid && order.userID != null)
                        {
                            if (order.customization == null)
                            {
                                order.customization = "";
                            }
                            if(order != null)
                            {
                                db.Orders.Add(order);
                                db.SaveChanges();
                            }
                            else
                            {
                                return Json(new { error = "Order was somehow null." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { error = "Invalid model state." }, JsonRequestBehavior.AllowGet);
                        }
                        ViewBag.orderID = new SelectList(db.Orders, "orderID", "userID", "menuItemID", RouteData.Values["id"]);
                        return Json(order);
                    }
                    else
                    {
                        return Json(new { error = "Invalid menu item id." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { error = "Invalid menu item id." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { error = "You need to be logged in to order." }, JsonRequestBehavior.AllowGet);
            }
        }

        //public ActionResult RemoveFromOrder()
        //{
        //    ViewBag.orderID = new SelectList(db.Orders, "orderID", "userID", "menuItemID", RouteData.Values["id"]);

        //    return View();
        //}

        [HttpPost]
        public JsonResult RemoveFromOrder([Bind(Include = "orderID")] Order order)
        {
            if (ModelState.IsValid)
            {
                Order ord = db.Orders
                    .Where(x => x.orderID == order.orderID)
                    .FirstOrDefault();
                if(ord != null)
                {
                    IPrincipal user = System.Web.HttpContext.Current.User;
                    if ((user != null) && user.Identity.IsAuthenticated)
                    {
                        if(user.Identity.GetUserId() != ord.userID)
                        {
                            return Json(new { success = "User ID for order and logged in user do not match." }, JsonRequestBehavior.AllowGet);
                        }
                        db.Orders.Remove(ord);
                        db.SaveChanges();
                        return Json(new { success = "Order removed sucessfully." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { error = "Need to be logged in to remove orders." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { error = "Order to remove is somehow null." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { error = "Model state is not valid." }, JsonRequestBehavior.AllowGet);
            }
        }

       [HttpPost]
        public ActionResult Vote(FormCollection t)
        {
            DateTime current = DateTime.Now;
            //[Bind(Include = "voteEndDate")] Manage manage;
            var first = db.Manages.ToList().First(); //new DateTime(2017, 3, 13, 10, 00, 00);
                                                     //DateTimePicker f = new DateTimePicker();
                                                     //d DateTime f = 
            int comp = DateTime.Compare(current, first.voteEndDate);
            //int comp = DateTime.Compare(current, first);
            if(comp > 0)
            {
                ViewBag.Message = "Voting not allowed after " + first.voteEndDate.ToString("f");
            }
            else
            {
                string userID = "";
                IPrincipal user = System.Web.HttpContext.Current.User;
                if (((user != null) && user.Identity.IsAuthenticated))
                {
                    userID = user.Identity.GetUserId();
                }
                if (userID.Length > 0)
                {
                    var query = from item in db.Users
                                where item.Id.Contains(userID)
                                select item;
                    var rowToChange = query.FirstOrDefault();
                    //First Choice in Vote
                    int firstVote = Int32.Parse(t["Choice1"].ToString());

                    //Second Choice in Vote
                    int secondVote = Int32.Parse(t["Choice2"].ToString());
                    //Third Choice in Vote
                    int thirdVote = Int32.Parse(t["Choice3"].ToString());

                    //All votes empty
                    if (firstVote == -1 && secondVote == -1 && thirdVote == -1)
                    {
                        ViewBag.Message = "Please choose a restaurant for at least one box.";
                    }
                    else if (firstVote != -1 && secondVote == -1 && thirdVote == -1)
                    {
                        //First voted on
                        rowToChange.FirstChoice = firstVote;
                        rowToChange.SecondChoice = -1;
                        rowToChange.ThirdChoice = -1;

                        db.SaveChanges();
                        ViewBag.Message = "Vote successfully cast.";
                    }
                    else if (firstVote == -1 && secondVote != -1 && thirdVote == -1)
                    {
                        //Second voted on
                        rowToChange.FirstChoice = -1;
                        rowToChange.SecondChoice = secondVote;
                        rowToChange.ThirdChoice = -1;

                        db.SaveChanges();
                        ViewBag.Message = "Vote successfully cast.";
                    }
                    else if (firstVote == -1 && secondVote == -1 && thirdVote != -1)
                    {
                        //Third voted on
                        rowToChange.FirstChoice = -1;
                        rowToChange.SecondChoice = -1;
                        rowToChange.ThirdChoice = thirdVote;

                        db.SaveChanges();
                        ViewBag.Message = "Vote successfully cast.";
                    }
                    else if (firstVote != -1 && secondVote != -1 && thirdVote == -1)
                    {
                        //First and Second voted on
                        if (firstVote == secondVote)
                        {
                            ViewBag.Message = "Duplicate votes are not allowed.";
                        }
                        else
                        {
                            rowToChange.FirstChoice = firstVote;
                            rowToChange.SecondChoice = secondVote;
                            rowToChange.ThirdChoice = -1;

                            db.SaveChanges();
                            ViewBag.Message = "Vote successfully cast.";
                        }
                    }
                    else if (firstVote != -1 && secondVote == -1 && thirdVote != -1)
                    {
                        //First and Third voted on
                        if (firstVote == thirdVote)
                        {
                            ViewBag.Message = "Duplicate votes are not allowed.";
                        }
                        else
                        {
                            rowToChange.FirstChoice = firstVote;
                            rowToChange.SecondChoice = -1;
                            rowToChange.ThirdChoice = thirdVote;

                            db.SaveChanges();
                            ViewBag.Message = "Vote successfully cast.";
                        }
                    }
                    else if (firstVote == -1 && secondVote != -1 && thirdVote != -1)
                    {
                        //Second and Third voted on
                        if (secondVote == thirdVote)
                        {
                            ViewBag.Message = "Duplicate votes are not allowed.";
                        }
                        else
                        {
                            rowToChange.FirstChoice = -1;
                            rowToChange.SecondChoice = secondVote;
                            rowToChange.ThirdChoice = thirdVote;

                            db.SaveChanges();
                            ViewBag.Message = "Vote successfully cast.";
                        }
                    }
                    else
                    {
                        //Check for duplicate votes
                        if (firstVote == secondVote || firstVote == thirdVote || secondVote == thirdVote)
                        {
                            ViewBag.Message = "Duplicate votes are not allowed.";
                        }
                        else
                        {
                            rowToChange.FirstChoice = firstVote;
                            rowToChange.SecondChoice = secondVote;
                            rowToChange.ThirdChoice = thirdVote;
                            db.SaveChanges();

                            ViewBag.Message = "Vote successfully cast.";
                        }
                    }
                }

                else
                {
                    ViewBag.Message = "Please log in to vote.";
                }
            }
            
            
            ViewData["TopScoresList"] = getTopScores();
            ViewData["RestaurantList"] = db.Restaurants.ToList();
            ViewData["MenuList"] = db.Menus.ToList();
            ViewData["MenuItemList"] = db.MenuItems.ToList();
            ViewData["OrderList"] = db.Orders.ToList();
            return View("Index");
        }
    }
}