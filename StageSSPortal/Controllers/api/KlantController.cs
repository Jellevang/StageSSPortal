using BL;
using Domain;
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
        SSHManager sshMgr = new SSHManager();

        [HttpPost]
        [Route("api/klant/AddKlant/{naam}/{email}/{afk}")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult addKlant(string naam, string email, string afk)
        {
            mgr.AddKlant(naam, email, afk);
            return Ok();

        }
        [HttpGet]
        [Route("api/Klant/GetKlantOvms/{klantEmail}")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult GetOvmsKlant(string klantEmail)
        {
            try
            {
                Klant k = mgr.GetKlant(klantEmail);
                List<OracleVirtualMachine> ovms = sshMgr.GetKlantOVMs(k.KlantId).ToList() ;
                return Ok(ovms);
            }
            catch
            {
                return Ok();
            }

        }
    }
}
