using System;
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
    public class MenuItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private SecurityHelper sh = new SecurityHelper();

        // GET: MenuItems
        public ActionResult Index()
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                var menuItems = db.MenuItems.Include(m => m.Menu);
                return View(menuItems.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: MenuItems/Details/5
        public ActionResult Details(int? id)
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                MenuItem menuItem = db.MenuItems.Find(id);
                if (menuItem == null)
                {
                    return HttpNotFound();
                }
                return View(menuItem);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: MenuItems/Create
        public ActionResult Create()
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                ViewBag.menuID = new SelectList(db.Menus, "menuID", "menuName", RouteData.Values["id"]);
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: MenuItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "menuItemID,menuItemName,menuItemPrice,menuItemDescription,menuID,itemType")] MenuItem menuItem)
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                if (ModelState.IsValid)
                {
                    db.MenuItems.Add(menuItem);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Restaurants");
                }

                ViewBag.menuID = new SelectList(db.Menus, "menuID", "menuName", menuItem.menuID);
                return View(menuItem);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: MenuItems/Edit/5
        public ActionResult Edit(int? id)
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                MenuItem menuItem = db.MenuItems.Find(id);
                if (menuItem == null)
                {
                    return HttpNotFound();
                }
                ViewBag.menuID = new SelectList(db.Menus, "menuID", "menuName", menuItem.menuID);
                List<string> itemList = new List<string>();
                itemList.Add("Appetizer");
                itemList.Add("Entree");
                itemList.Add("Topping");
                itemList.Add("Free");
                ViewBag.itemType = new SelectList(itemList, db.MenuItems.Where(x => x.menuItemID == id).FirstOrDefault().itemType);
                return View(menuItem);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: MenuItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "menuItemID,menuItemName,menuItemPrice,menuItemDescription,menuID,itemType")] MenuItem menuItem)
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                if (ModelState.IsValid)
                {
                    db.Entry(menuItem).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Restaurants");
                }
                ViewBag.menuID = new SelectList(db.Menus, "menuID", "menuName", menuItem.menuID);
                return View(menuItem);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: MenuItems/Delete/5
        public ActionResult Delete(int? id)
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                MenuItem menuItem = db.MenuItems.Find(id);
                if (menuItem == null)
                {
                    return HttpNotFound();
                }
                return View(menuItem);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: MenuItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastAdmin(user))
            {
                MenuItem menuItem = db.MenuItems.Find(id);
                db.MenuItems.Remove(menuItem);
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
