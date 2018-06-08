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
        
        private readonly ISSHManager mgr = new SSHManager();
        private readonly IKlantManager klantmgr = new KlantManager();
        AdminManager admgr = new AdminManager();
        SshClient ssh;
        public SSHController()
        {
            Admin admin = admgr.GetAdmin();
            string passwd = admgr.GetPasswd(admin);
            string trimpasswd = passwd.Replace("'", "");
            ssh = new SshClient("10.0.12.240", 10000, "admin", trimpasswd);

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
        [HttpGet]
        [Route("KlantAccount/SSH/Account")]
        [Authorize(Roles = "KlantAccount")]
        public ActionResult Account()
        {
            return View();
        }

        [HttpGet]
        [Route("Admin/SSH/LogAdmin")]
        [Authorize(Roles = "Admin")]
        public ActionResult LogAdmin()
        {
            List<Klant> klanten = new List<Klant>();
            List<Klant> klantAccounts = new List<Klant>();
            List<Klant> klantenHoofd = klantmgr.GetHoofdKlanten().ToList();
            foreach (Klant hoofd in klantenHoofd)
            {
                klanten.Add(hoofd);
                klantAccounts = klantmgr.GetKlantenAccounts(hoofd).ToList();
                klanten.AddRange(klantAccounts);
            }

            return View(klanten);
        }

        [HttpGet]
        [Route("Klant/SSH/LogKlant/{id}")]
        [Authorize(Roles = "Klant")]
        public ActionResult LogKlant()
        {
            Klant k = klantmgr.GetKlant(User.Identity.Name);
            List<Klant> klanten = new List<Klant>();
            List<Klant> klantAccounts = new List<Klant>();
            try
            {
                Klant klantenHoofd = klantmgr.GetHoofdKlant(k.KlantId);
                klanten.Add(klantenHoofd);
                klantAccounts = klantmgr.GetKlantenAccounts(klantenHoofd).ToList();
                klanten.AddRange(klantAccounts);
            }
            catch
            {
                klanten.Add(k);
            }
            return View(klanten);
        }
    }
}