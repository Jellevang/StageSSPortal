using BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace StageSSPortal.Controllers.api
{
    public class KlantController : ApiController
    {
        KlantManager mgr = new KlantManager();

        [HttpPost]
        [Route("api/klant/AddKlant/{naam}/{email}/{afk}")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult addKlant(string naam, string email, string afk)
        {
            mgr.AddKlant(naam, email, afk);
            return Ok();

        }
    }
}
