using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcLunchSite.Models;
using System.Data.Entity.Infrastructure;
using MvcLunchSite.Helpers;
using System.Security.Principal;

namespace MvcLunchSite.Controllers
{
    public class MenusController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private SecurityHelper sh = new SecurityHelper();
        // GET: Menus
        public ActionResult Index()
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                return View(db.Menus.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Menus/Details/5
        public ActionResult Details(int? id)
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Menu menu = db.Menus.Find(id);
                if (menu == null)
                {
                    return HttpNotFound();
                }
                return View(menu);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Menus/Create
        public ActionResult Create()
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                ViewBag.restaurantID = new SelectList(db.Restaurants, "ID", "name", RouteData.Values["id"]);
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Menus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "menuID,restaurantID,menuName")] Menu menu)
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                if (ModelState.IsValid)
                {
                    db.Menus.Add(menu);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Restaurants");
                }
                ViewBag.restaurantID = new SelectList(db.Restaurants, "ID", "name");
                return View(menu);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Menus/Edit/5
        public ActionResult Edit(int? id)
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Menu menu = db.Menus.Find(id);
                if (menu == null)
                {
                    return HttpNotFound();
                }
                return View(menu);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Menus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "menuID,restaurantID,menuName")] Menu menu)
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                if (ModelState.IsValid)
                {
                    db.Entry(menu).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Restaurants");
                }
                return View(menu);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Menus/Delete/5
        public ActionResult Delete(int? id)
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Menu menu = db.Menus.Find(id);
                if (menu == null)
                {
                    return HttpNotFound();
                }
                return View(menu);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                Menu menu = db.Menus.Find(id);
                foreach (MenuItem item in db.MenuItems)
                {
                    if (item.menuID == id)
                    {
                        db.MenuItems.Remove(item);
                    }
                }
                db.Menus.Remove(menu);
                db.SaveChanges();
                return RedirectToAction("Index", "Restaurants");
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
