﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MvcLunchSite.Models;
using System.Data.Entity;
using MvcLunchSite.Helpers;
using System.Security.Principal;

namespace MvcLunchSite.Controllers
{

    [Authorize]
    public class ManageController : Controller
    {
        private SecurityHelper sh = new SecurityHelper();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                RoleName = RoleName()
            };
            ViewData["UserList"] = db.Users.ToList();

            //Manage t = new Manage();
            //t.voteEndDate = DateTime.Now.AddDays(1);
            //t.orderEndDate = DateTime.Now.AddDays(3);
            //t.Id = db.Manages.Count() + 1;
            //db.Manages.Add(t);
            //db.SaveChanges();
            ViewData["TimeList"] = db.Manages.ToList();

            if(TempData["errorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["errorMessage"];
            }

            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }
        
        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        public ActionResult ChangeRole()
        {
            var user = System.Web.HttpContext.Current.User;
            bool allowed = false;
            if ((user != null) && user.Identity.IsAuthenticated) {
                var identity_query = from item in db.Users
                                     where item.Role.Equals("superuser") && item.UserName.Equals(user.Identity.Name)
                                     select item;
                var test = identity_query.FirstOrDefault();
                if(test != null)
                {
                    allowed = true;
                }                     
            }
            if (allowed)
            {
                ViewBag.userBool = false;
                ViewBag.ordererBool = false;
                ViewBag.adminBool = false;
                ViewBag.superuserBool = false;
                ViewBag.InitialSelect = "user";
                if (Url.RequestContext.RouteData.Values["id"] != null)
                {
                    string userId = Url.RequestContext.RouteData.Values["id"].ToString();
                    var query = from item in db.Users
                                where item.Id.Equals(userId)
                                select item;
                    var queryItem = query.FirstOrDefault();
                    if (queryItem != null)
                    {
                        ViewBag.Item = queryItem.Email;
                        if (queryItem.Role != null)
                        {
                            ViewBag.InitialSelect = queryItem.Role.ToLower();
                        }
                    }
                }

                if (ViewBag.InitialSelect == "superuser")
                {
                    ViewBag.superuserBool = true;
                }
                else if (ViewBag.InitialSelect == "orderer")
                {
                    ViewBag.ordererBool = true;
                }
                else if (ViewBag.InitialSelect == "admin")
                {
                    ViewBag.adminBool = true;
                }
                else
                {
                    ViewBag.userBool = true;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult ChangeRole( ChangeRoleViewModel RoleView)
        public async Task<ActionResult> ChangeRole([Bind(Include = "email,RoleName")] IndexViewModel RoleView)
        {
            var requestUser = System.Web.HttpContext.Current.User;
            bool allowed = false;
            bool ownRoleChanger = false;
            if ((requestUser != null) && requestUser.Identity.IsAuthenticated)
            {
                var identity_query = from item in db.Users
                                     where item.Role.Equals("superuser") && item.UserName.Equals(requestUser.Identity.Name)
                                     select item;
                var test = identity_query.FirstOrDefault();
                if (test != null)
                {
                    if(RoleView.email != requestUser.Identity.Name)
                    {
                        allowed = true;
                    }
                    else
                    {
                        ownRoleChanger = true;
                    }
                }
            }
            if (allowed)
            {
                if (ModelState.IsValid)
                {
                    // db.Entry(RoleView).State = EntityState.Modified;
                    //db.SaveChanges();
                    //return RedirectToAction("Index");
                    var user = await UserManager.FindByEmailAsync(RoleView.email);
                    if (user != null)
                    {

                        foreach (ApplicationUser use in db.Users)
                        {
                            if (use.Email == RoleView.email)
                            {
                                use.Role = RoleView.RoleName;
                            }
                        }
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                return View(RoleView);
            }
            else
            {
                if (ownRoleChanger)
                {
                    TempData["errorMessage"] = "You can't change your own role.";
                }
                else
                {
                    TempData["errorMessage"] = "There was problem with changing the role of the user. It may be due to authorization or authentication.";
                }
                return RedirectToAction("Index");
            }
        }
        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        public ActionResult Time()
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastSuperuser(user))
            {
                return View("~/Views/Manage/Time.cshtml");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Time(FormCollection t)
        {
            IPrincipal user = System.Web.HttpContext.Current.User;
            if (sh.atLeastSuperuser(user))
            {
                string voteEnd = t["voteEndDate"];
                string orderEnd = t["orderEndDate"];

                DateTime date = Convert.ToDateTime(voteEnd);
                DateTime orderDate = Convert.ToDateTime(orderEnd);
                var manages = db.Manages.First();
                manages.voteEndDate = date;
                manages.orderEndDate = orderDate;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        //public ActionResult clearVotes()
        //{
        //    IPrincipal user = System.Web.HttpContext.Current.User;
        //    if (sh.atLeastSuperuser(user))
        //    {
        //        return View("~/Views/Manage/Time.cshtml");
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult clearVotes()
        {
            IPrincipal requestingUser = System.Web.HttpContext.Current.User;
            if (sh.atLeastSuperuser(requestingUser))
            {
                DateTime today = DateTime.Today;
                DateTime futureOrder = today;
                DateTime futureVote = today;
                int month = 1;
                //int day = 1;
                int year = today.Year;
                if(today.Day>=7 || today.DayOfWeek==DayOfWeek.Friday)
                {
                    if (today.Month == 12)
                        year = today.Year + 1;
                    else
                        month = today.Month;
                    futureOrder = new DateTime(year, month+1, 1, 10, 00, 00);
                    futureVote = new DateTime(year, month, DateTime.DaysInMonth(year,month), 17, 00, 00);
                    if (futureVote.DayOfWeek != DayOfWeek.Thursday)
                    {
                        futureVote = futureOrder;
                        for (int a = 1; a <= 7; a++)
                        {
                            futureVote.AddDays(1);
                            if (futureVote.DayOfWeek == DayOfWeek.Thursday)
                                break;
                        }
                    }

                    if (futureOrder.DayOfWeek != DayOfWeek.Friday)
                    {
                        for(int a = 2; a <= 7; a++)
                        {
                            futureOrder.AddDays(1);
                            if (futureOrder.DayOfWeek == DayOfWeek.Friday)
                                break;
                        }
                    }
                    
                }

                if (futureOrder != today)
                {
                    db.Manages.First().orderEndDate = futureOrder;
                    db.Manages.First().voteEndDate = futureVote;
                }

                foreach (var user in db.Users)
                {
                    user.FirstChoice = null;
                    user.SecondChoice = null;
                    user.ThirdChoice = null;
                }
                db.SaveChanges();
                return RedirectToAction("Index", "Manage");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

#region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }
        private string RoleName()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.Role;
            }
            return "";
        }
        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}