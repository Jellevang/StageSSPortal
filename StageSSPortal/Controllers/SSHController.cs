using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Renci.SshNet;
using StageSSPortal.Models;
using Domain;
using BL;
using DAL.EF;
using Microsoft.AspNet.Identity;

namespace StageSSPortal.Controllers
{
    public class SSHController : Controller
    {
        // GET: SSH
        private readonly ISSHManager mgr = new SSHManager();
        private readonly IKlantManager klantmgr = new KlantManager();
        SshClient ssh;
        public SSHController()
        {
            ssh = new SshClient("10.0.12.240", 10000, "admin", "tst0VMman");

        }

        [HttpGet]
        [Route("Admin/SSH/StartSSHOVM")]
        [Authorize(Roles = "Admin")]
        public ActionResult StartSSHOVM()
        {
            return View();
        }
        [HttpGet]
        [Route("Admin/SSH/KlantOVMs")]
        [Authorize(Roles = "Klant")]
        public ActionResult KlantOVMs()
        {
            return View();
        }
    }
}



