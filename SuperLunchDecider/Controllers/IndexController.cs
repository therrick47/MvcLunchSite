using SuperLunchDecider.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SuperLunchDecider.Controllers
{
    public class IndexController : Controller
    {
        // GET: Index
        public ActionResult Index()
        {
            ViewBag.Message = "";
            dynamic mymodel = new ExpandoObject();
            mymodel.Restaurants = GetRestaurants();
            return View(mymodel);
        }
        public List<Restaurant> GetRestaurants()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<Restaurant> restaurants = new List<Restaurant>();
            foreach (Restaurant r in db.Restaurants)
                restaurants.Add(r);
            return restaurants;
        }
    }
}