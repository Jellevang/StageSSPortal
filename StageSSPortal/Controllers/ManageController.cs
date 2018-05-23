using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using BL;
using StageSSPortal.Models;
using StageSSPortal.Helpers;
using Domain;

namespace StageSSPortal.Controllers
{
    [Authorize]
    public partial class ManageController : Controller
    {
        private SignInManager _signInManager;
        private GebruikerManager _userManager;
        private readonly IKlantManager mgr = new KlantManager();
        public ManageController()
        {
            _userManager = GebruikerManager.Create(System.Web.HttpContext.Current.GetOwinContext().Get<AppBuilderProvider>().Get().GetDataProtectionProvider()); // AppbuilerProvider is een custom klasse die geregistreerd wordt in de startup.auth.cs
            _signInManager = SignInManager.Create(_userManager, System.Web.HttpContext.Current.GetOwinContext());
        }

        public ManageController(GebruikerManager userManager, SignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public SignInManager SignInManager
        {
            get { return _signInManager; }
            private set
            {
                _signInManager = value;
            }
        }

        public GebruikerManager UserManager
        {
            get { return _userManager; }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public virtual async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Uw wachtwoord is veranderd."
                : message == ManageMessageId.SetPasswordSuccess ? "Uw wachtwoord is ingesteld."
                : message == ManageMessageId.Error ? "Er is iets foutgelopen."
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
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

        //[HttpPost]
        //public ActionResult ChangeOvmPassword(ChangePasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
            
        //    Admin a = adm.GetAdmin();
        //    if (adm.GetPasswd(a) == model.OldPassword)
        //    {
        //        adm.UpdatePasswd(model.NewPassword, a);
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "Het oude password is niet correct!");

        //    }
        //    return View();
        //}

        // GET: /Manage/ChangeOvmPassword
        public virtual ActionResult ChangeOvmPassword()
        {
            return View();
        }
        //
        // GET: /Manage/ChangePassword
        public virtual ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
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
                if(user.MustChangePassword == true)
                {
                    user.MustChangePassword = false;
                    user.LastPasswordChangedDate = DateTime.Now;
                    UserManager.UpdateGebruiker(user);
                }
                return RedirectToAction("Index", "Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public virtual ActionResult ResetKlant()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public virtual async Task<ActionResult> ResetKlant(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
               
                ModelState.AddModelError("", "De opgegeven klant bestaat niet");
                return View("ResetKlant");
            }
            string resetToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, resetToken, model.NewPassword);
            if (result.Succeeded)
            {
                if(user.Toegestaan == false)
                {
                    mgr.UnblockKlant(user.GebruikerId);
                }
                user.MustChangePassword = true;
                user.LastPasswordChangedDate = DateTime.Now;
                UserManager.UpdateGebruiker(user);
                return RedirectToAction("Index", "Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View();
        }

        [Authorize(Roles = "Klant")]
        public virtual ActionResult ResetKlantAccount()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Klant")]
        public virtual async Task<ActionResult> ResetKlantAccount(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Klant k = mgr.GetKlant(User.Identity.GetUserName());
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                ModelState.AddModelError("", "De opgegeven medewerker bestaat niet");
                return View("ResetKlantAccount");
            }
            Klant sub = mgr.GetKlant(model.Email);
            if (sub.IsKlantAccount==false)
            {
                ModelState.AddModelError("", "De opgegeven medewerker bestaat niet");
                return View("ResetKlantAccount");
            }
            if (sub.HoofdKlant != k)
            {
                ModelState.AddModelError("", "De opgegeven medewerker bestaat niet");
                return View("ResetKlantAccount");
            }
            if (sub.HoofdKlant.Equals(k))
            {
                string resetToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var result = await UserManager.ResetPasswordAsync(user.Id, resetToken, model.NewPassword);
                if (result.Succeeded)
                {
                    if (user.Toegestaan == false)
                    {
                        mgr.UnblockKlantAccount(user.GebruikerId);
                    }
                    user.MustChangePassword = true;
                    user.LastPasswordChangedDate = DateTime.Now;
                    UserManager.UpdateGebruiker(user);
                    return RedirectToAction("Index", "Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                AddErrors(result);
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Manage");
            }
            
        }

        //
        // GET: /Manage/SetPassword
        public virtual ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> SetPassword(SetPasswordViewModel model)
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
            return View(model);
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