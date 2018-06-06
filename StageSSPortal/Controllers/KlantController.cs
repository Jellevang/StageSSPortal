using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Helpers;
using BL;
using Domain.Gebruikers;
using DAL.EF;
using Domain;
using System.Text.RegularExpressions;

namespace StageSSPortal.Controllers
{
    public class KlantController : Controller
    {
        private readonly IKlantManager mgr = new KlantManager();
        private readonly ISSHManager sshmgr = new SSHManager();
        private readonly UserManager<Gebruiker> userManager = new UserManager<Gebruiker>(new UserStore<Gebruiker>(new StageSSPortalDbContext()));
        public virtual ActionResult Klanten()
        {
            return View();
        }
        [Route("Klant/Home")]
        [Authorize(Roles = "Klant")]
        public ActionResult Home()
        {
            ViewBag.Melding = TempData["Melding"];
            return View();
        }
        // GET: Klant
        [Route("Admin/Klant")]
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            //IEnumerable<Klant> Klanten = mgr.GetKlanten();
            List<Klant> klanten = new List<Klant>();
            List<Klant> klantAccounts = new List<Klant>();
            List<Klant> klantenHoofd = mgr.GetHoofdKlanten().ToList();
            foreach(Klant hoofd in klantenHoofd)
            {
                klanten.Add(hoofd);
                klantAccounts = mgr.GetKlantenAccounts(hoofd).ToList();
                klanten.AddRange(klantAccounts);
            }

            return View(klanten);
        }

        // POST
        [Route("Admin/Klant/Invite")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Invite(Klant Klant)
        {
            if (ModelState.IsValid)
            {
                //Klant = mgr.InviteKlant(Klant.Email);
                return RedirectToAction("Details", new { id = Klant.KlantId });
            }
            return View();
        }

        [Route("Admin/Klant/Create")]
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {

            return View();
        }

        //// POST
        [Route("Admin/Klant/Create")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(Klant Klant, FormCollection collection)
        {
            
            if (Klant.Naam == null || Klant.Naam == "")
            {
               // ModelState.AddModelError("", "Geef een gebruikersnaam in");
                return View("Create");
            }
            if (Klant.Email == null || Klant.Email == "")
            {
                return View("Create");
            }
            //if (Klant.Afkorting == null || Klant.Afkorting == "")
            //{
            //    ViewBag.Message = "Bent u zeker dat u geen afkorting wilt meegeven?";
            //    return View("Create");
            //}

           
                
            
            
            Klant email = mgr.GetKlant(Klant.Email);
            if (email != null)
            {
                ModelState.AddModelError("", "Email moet uniek zijn");
                return View("Create");
            }
            string emailregex = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            var regex = Regex.Match(Klant.Email, emailregex);
            if (regex.Length == 0)
            {
                ModelState.AddModelError("", "Email voldoet niet aan de voorwaarde bv: test@monin.be");
                return View("Create");
            }
            Klant naam = mgr.GetKlantByName(Klant.Naam);
            if (naam != null && naam.IsKlantAccount == false)
            {
                ModelState.AddModelError("", "Er is al reeds een klant met deze naam enkel accounts mogen dezelfde naam hebben");
                return View("Create");
            }
            else
            {
                if (Klant.Afkorting == "" || Klant.Afkorting == null)
                {
                    Klant.Afkorting = Klant.Naam.ToUpper();
                }
                else {
                    List<Klant> klanten = mgr.GetKlanten().ToList();
                    foreach (var k in klanten)
                    {
                        if (k.Afkorting.Equals(Klant.Afkorting.ToUpper()))
                        {
                            ModelState.AddModelError("", "Afkorting moet uniek zijn");
                            return View("Create");
                        }
                    }
                }
                Klant = mgr.AddKlant(Klant.Naam, Klant.Email,Klant.Afkorting.ToUpper());
                return RedirectToAction("Details", new { id = Klant.KlantId });
            }

        }

        [Route("Admin/Klant/Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {

            Klant Klant = mgr.GetKlant(id);
            return View(Klant);
        }

        // POST
        [HttpPost]
        [Route("Admin/Klant/Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, FormCollection collection)
        {
            
            
            Klant k = mgr.GetKlant(id);
            if(k.IsKlantAccount==false)
            {
                List<OracleVirtualMachine> ovms = sshmgr.GetKlantOVMs(id).ToList();
                for (int i = 0; i < ovms.Count(); i++)
                {
                    ovms[i].KlantId = 0;
                    sshmgr.ChangeOVM(ovms[i]);
                }
                List<Klant> accs = mgr.GetKlantenAccounts(k).ToList();
                for(int i =0;i<accs.Count();i++)
                {
                    sshmgr.RemoveLijstenAccount(accs[i].KlantId);
                }
            }
            else
            {
                sshmgr.RemoveLijstenAccount(k.KlantId);
            }
            mgr.RemoveKlant(id);
            return RedirectToAction("Index");
        }

        // GET
        [Route("Admin/Klant/Details/{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int id)
        {
            Klant Klant = mgr.GetKlant(id);
            return View(Klant);
        }

        // GET
        [Route("Admin/Klant/Edit/{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            Klant Klant = mgr.GetKlant(id);
            return View(Klant);
        }

        //POST
        [Route("Admin/Klant/Edit/{id}")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Edit(Klant Klant)
        {
            if (ModelState.IsValid)
            {
               if(Klant.IsKlantAccount==true)
                {
                    Klant.IsKlantAccount = true;
                    mgr.ChangeKlant(Klant);
                }
                else
                {
                    Klant origineel = mgr.GetKlant(Klant.KlantId);
                    List<Klant> klanten = mgr.GetKlanten().ToList();

                    foreach (var k in mgr.GetKlanten().ToList())
                    {
                        if (k.KlantId == origineel.KlantId)
                        {
                            klanten.Remove(k);
                        }
                    }
                    foreach (var k in klanten)
                    {
                        if (Klant.Naam == k.Naam)
                        {
                            ModelState.AddModelError("", "Geef een nieuwe naam op");
                            return View("Edit");
                        }
                        if (Klant.Email == k.Email)
                        {
                            ModelState.AddModelError("", "Geef een nieuwe email op");
                            return View("Edit");
                        }
                        if (Klant.Afkorting == k.Afkorting)
                        {
                            ModelState.AddModelError("", "Geef een nieuwe afkorting op");
                            return View("Edit");
                        }
                    }
                    //    mgr.RemoveKlant(origineel.KlantId);
                    mgr.ChangeKlant(Klant);
                }                
                return RedirectToAction("Index");
            }
            return View();
        }
        [Route("Admin/Klant/EditAccount/{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult EditAccount(int id)
        {
            Klant Klant = mgr.GetKlant(id);
            return View(Klant);
        }

        //POST
        [Route("Admin/Klant/EditAccount/{id}")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditAccount(Klant Klant)
        {
            if (ModelState.IsValid)
            {
                Klant.IsKlantAccount = true;
                mgr.ChangeKlant(Klant);
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET
        [Route("Admin/Klant/Block/{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Block(int id)
        {
            Klant Klant = mgr.GetKlant(id);
            return View(Klant);
        }

        // POST
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Admin/Klant/Block/{id}")]
        public ActionResult Block(int id, FormCollection collection)
        {
            mgr.BlockKlant(id);
            return RedirectToAction("Index");
        }

        // GET
        [Route("Admin/Klant/Unblock/{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Unblock(int id)
        {
            Klant Klant = mgr.GetKlant(id);
            return View(Klant);
        }

        // POST
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Admin/Klant/Unblock/{id}")]
        public ActionResult Unblock(int id, FormCollection collection)
        {
            mgr.UnblockKlant(id);
            return RedirectToAction("Index");
        }
    }
}