using BL;
using Domain;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace StageSSPortal.Controllers.api
{
    public class KlantAccountController : ApiController
    {
        private readonly IKlantManager mgr = new KlantManager();
        // GET: KlantAccount
        [HttpGet]
        [Route("api/Klant/KlantAccounts")]
        [Authorize(Roles = "Klant")]
        public IHttpActionResult GetKlantAccounts()
        {
            Klant k = mgr.GetKlant(User.Identity.GetUserName());
            IEnumerable<Klant> Klanten = mgr.GetKlantenAccounts(k);
            return Ok(Klanten);
        }
    }
}