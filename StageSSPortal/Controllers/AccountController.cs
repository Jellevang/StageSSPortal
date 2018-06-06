using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StageSSPortal.Models;
using StageSSPortal.Helpers;
using BL;
using Domain.Gebruikers;
using Microsoft.AspNet.Identity;
using Domain;

namespace StageSSPortal.Controllers
{
    [Authorize]
    public partial class AccountController : Controller
    {
        private SignInManager _signInManager;
        private GebruikerManager _userManager;
        private readonly IKlantManager mgr = new KlantManager();
        public AccountController()
        {
            _userManager = GebruikerManager.Create(System.Web.HttpContext.Current.GetOwinContext().Get<AppBuilderProvider>().Get().GetDataProtectionProvider()); // AppbuilerProvider is een custom klasse die geregistreerd wordt in de startup.auth.cs
            _signInManager = SignInManager.Create(_userManager, System.Web.HttpContext.Current.GetOwinContext());
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
        // GET: /Account/Login
        [AllowAnonymous]
        public virtual ActionResult Login(string returnUrl)
        {
            if (SignInManager.AuthenticationManager.User != null && SignInManager.AuthenticationManager.User.Identity != null && SignInManager.AuthenticationManager.User.Identity.IsAuthenticated == true)
            {
                //ViewBag.ReturnUrl = "/Account/Logoff";
                // LogOff();
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Login", "Account");

            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }           
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    if (user.LastPasswordChangedDate.AddMonths(6) < DateTime.Now && user.Rol != RolType.Admin)
                    {
                        //mgr.BlockKlant(user.GebruikerId);
                        user.MustChangePassword = true;
                        UserManager.UpdateGebruiker(user);
                        mgr.BlockKlant(user.GebruikerId);
                        ModelState.AddModelError("", "Uw passwoord is expired. Contacteer uw admin.");
                        return View("Login");
                    }
                if (user.LastPasswordChangedDate.AddDays(7) < DateTime.Now && user.Rol != RolType.Admin)
                {
                    //mgr.BlockKlant(user.GebruikerId);
                    ViewBag.Melding = "Passwoord vervalt binnen 7 dagen!";
                    return View(model);
                }
                if (user.Toegestaan == false)
                    {
                        ModelState.AddModelError("", "Uw account is geblokkeerd. Contacteer uw admin.");                        
                        return View("Login");
                    }
                    var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                    switch (result)
                    {
                        case SignInStatus.Success:
                            
                            if (user.Rol == RolType.KlantAccount)
                            {
                                if (user.MustChangePassword == true)
                                {
                                    returnUrl = "~/Manage/ChangePassword";
                                    return RedirectToLocal(returnUrl);
                                }
                                else
                                {
                                    returnUrl = "~/Home/Index";
                                    return RedirectToLocal(returnUrl);
                                }
                             }
                            if (user.Rol == RolType.Admin)
                            {
                                if (user.MustChangePassword == true)
                                {
                                    returnUrl = "~/Manage/ChangePassword";
                                    return RedirectToLocal(returnUrl);
                                }
                                else
                                {
                                    returnUrl = "~/Admin/Index";
                                    return RedirectToLocal(returnUrl);
                                }                           
                            }
                            if (user.MustChangePassword == true)
                            {
                                returnUrl = "~/Manage/ChangePassword";
                                return RedirectToLocal(returnUrl);
                            }
                            else
                            {
                                returnUrl = "~/Klant/Home";
                                return RedirectToLocal(returnUrl);
                            }                      
                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        case SignInStatus.RequiresVerification:
                            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                        case SignInStatus.Failure:

                        default:
                            ModelState.AddModelError("", "Invalid login attempt.");
                            return View(model);
                    }
                }
                ModelState.AddModelError("", "Ongeldige inlog gegevens");
          // TempData["Melding"] = "Passwoord vervalt binnen 7 dagen!";
            return View(model); 
        }
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
        #region Helpers
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}