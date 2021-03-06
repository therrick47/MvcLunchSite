﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcLunchSite.Models;
using MvcLunchSite.Helpers;
using System.Security.Principal;

namespace MvcLunchSite.Controllers
{
    public class RestaurantsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationDbContext db2 = new ApplicationDbContext();
        private SecurityHelper sh = new SecurityHelper();
        // GET: Restaurants
        public ActionResult Index()
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                ViewData["RestaurantList"] = db.Restaurants.ToList();
                ViewData["MenuList"] = db.Menus.ToList();
                ViewData["MenuItemList"] = db.MenuItems.ToList();

                //ViewBag.Restaurants = db.Restaurants.ToList();
                //ViewBag.Menus = db.Menus.ToList();
                //ViewBag.MenuItems = db.MenuItems.ToList();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }

        // GET: Restaurants/Details/5
        public ActionResult Details(int? id)
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Restaurant restaurant = db.Restaurants.Find(id);
                if (restaurant == null)
                {
                    return HttpNotFound();
                }
                return View(restaurant);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Restaurants/Create
        public ActionResult Create()
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                ViewBag.restaurantID = new SelectList(db.Restaurants, "ID", "name");
                //ViewBag.menuID = new SelectList(db.Menus, "menuID", "menuName");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Restaurants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,name")] Restaurant restaurant)
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                if (ModelState.IsValid)
                {
                    db.Restaurants.Add(restaurant);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.restaurantID = new SelectList(db.Restaurants, "ID", "name");
                //ViewBag.menuID = new SelectList(db.Menus, "menuID", "menuName");
                return View(restaurant);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Restaurants/Edit/5
        public ActionResult Edit(int? id)
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Restaurant restaurant = db.Restaurants.Find(id);
                if (restaurant == null)
                {
                    return HttpNotFound();
                }
                return View(restaurant);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Restaurants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,name")] Restaurant restaurant)
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                if (ModelState.IsValid)
                {
                    db.Entry(restaurant).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(restaurant);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Restaurants/Delete/5
        public ActionResult Delete(int? id)
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Restaurant restaurant = db.Restaurants.Find(id);
                if (restaurant == null)
                {
                    return HttpNotFound();
                }
                return View(restaurant);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Restaurants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                Restaurant restaurant = db.Restaurants.Find(id);
                foreach (var item in db.Menus)
                {
                    if (item.restaurantID == restaurant.ID)
                    {

                        foreach (var menuItem in db2.MenuItems)
                        {
                            if (item.menuID == menuItem.menuID)
                            {
                                db.MenuItems.Attach(menuItem);
                                db.Entry(menuItem).State = EntityState.Deleted;
                                //db.MenuItems.Remove(menuItem);
                            }
                        }
                        db.Menus.Remove(item);
                    }
                }
                db.Restaurants.Remove(restaurant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

}
