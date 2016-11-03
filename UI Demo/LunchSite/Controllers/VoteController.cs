using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using LunchSite.Models;

//using VoteControl.Models;

namespace LunchSite.Controllers
{
    public class VoteController : Controller
    {
        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public ActionResult DoVote(string uniqueName, int voteId)
        {
            ActionResult result;

            VoteDataModel model = new VoteDataModel(uniqueName, Request.PhysicalApplicationPath + "App_Data\\");
            if (model.Open())
            {
                string ipAddr = Request.UserHostAddress;

                if (voteId < model.Answers.Count)
                {
                    if (model.DoVote(voteId, ipAddr))
                    {
                        String str = RenderPartialViewToString("VoteControl", model);
                        int start = str.IndexOf('>');
                        int end = str.LastIndexOf('<');
                        str = str.Substring(start + 1, end - start - 1);    //strip start and end div tags
                        result = Content(str);
                    }
                    else
                    {
                        result = Content("*You have already voted.");
                    }
                }
                else
                {
                    result = Content("*Invalid vote ID.");
                }
            }
            else
            {
                result = Content("*Poll does not exist");
            }
            return result;
        }



    }
}
