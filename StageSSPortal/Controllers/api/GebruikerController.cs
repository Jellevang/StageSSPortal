using BL;
using StageSSPortal.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Domain.Gebruikers;

namespace StageSSPortal.Controllers.api
{
    public class GebruikerController : ApiController
    {
        private SignInManager _signInManager;
        private GebruikerManager _userManager;

        public GebruikerController()
        {
            _userManager = GebruikerManager.Create(System.Web.HttpContext.Current.GetOwinContext().Get<AppBuilderProvider>().Get().GetDataProtectionProvider()); // AppbuilderProvider is een custom klasse die geregistreerd wordt in de startup.auth.cs
        }
        [HttpGet]
        [Route("api/Gebruiker/GetGebruiker")]
        public IHttpActionResult GetGebruiker()
        {
            Gebruiker gebruiker =_userManager.GetGebruiker(User.Identity.Name);
            return Ok(gebruiker);
        }
    }
}
