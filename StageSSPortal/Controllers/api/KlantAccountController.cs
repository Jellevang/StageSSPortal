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
        private readonly ISSHManager SshMgr = new SSHManager();

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
        [HttpGet]
        [Route("api/Klant/OvmLijst/{klantId}/{ovmId}")]
        [Authorize(Roles = "Klant")]
        public IHttpActionResult GetOvmLijst(int klantId, string ovmId)
        {
            try
            {
                OVMLijst ovm = SshMgr.GetLijst(klantId, ovmId);
                return Ok(ovm);
            }
            catch
            {
                return Ok();
            }
           
        }

        [HttpGet]
        [Route("api/Klant/OvmLijstKlant/{klantEmail}/{ovmId}")]
        [Authorize(Roles = "Klant")]
        public IHttpActionResult GetOvmLijstKlant(string klantEmail, string ovmId)
        {
            try
            {
                Klant k = mgr.GetKlant(klantEmail);
                OVMLijst ovm = SshMgr.GetLijst(k.KlantId, ovmId);
                return Ok(ovm);
            }
            catch
            {
                return Ok();
            }

        }
        [HttpGet]
        [Route("api/Klant/OvmsKlant/{klantEmail}")]
        [Authorize(Roles = "Klant")]
        public IHttpActionResult GetOvmsKlant(string klantEmail)
        {
            try
            {
                List<OracleVirtualMachine> ovms = new List<OracleVirtualMachine>();
                Klant k = mgr.GetKlant(klantEmail);
                IEnumerable<OVMLijst> lijst = SshMgr.GetLijstAccount(k.KlantId);
                foreach(var v in lijst.ToList())
                {
                    ovms.Add(SshMgr.GetOVMById(v.OVMId));
                }
                return Ok(ovms);
            }
            catch
            {
                return Ok();
            }

        }

    }
}