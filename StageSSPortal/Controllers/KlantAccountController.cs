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

namespace StageSSPortal.Controllers
{
    public class KlantAccountController : Controller
    {
        // GET: KlantAccount
        
        private readonly IKlantManager mgr = new KlantManager();
        private readonly ISSHManager sshmgr = new SSHManager();
        private readonly UserManager<Gebruiker> userManager = new UserManager<Gebruiker>(new UserStore<Gebruiker>(new StageSSPortalDbContext()));
        public virtual ActionResult KlantenAccounten()
        {
            return View();
        }

        // GET: Klant
        [Route("Klant/KlantAccount/")]
        [Authorize(Roles = "Klant")]
        public ActionResult Index()
        {
            Klant k = mgr.GetKlant(User.Identity.GetUserName());
            IEnumerable<Klant> Klanten = mgr.GetKlantenAccounts(k);
            return View(Klanten);
        }

        // GET: Klant-informatie
        [Route("KlantAccount/Profiel/")]
        [Authorize(Roles = "KlantAccount")]
        public ActionResult Profiel()
        {
            var user = userManager.FindById(User.Identity.GetUserId());
            ViewBag.Id = user.GebruikerId;
            return View();
        }



        [Route("Klant/Profiel/Create")]
        [Authorize(Roles = "KlantAccount")]
        [HttpPost]
        public ActionResult CreateProfiel(Klant Klant, FormCollection collection)
        {
            WebImage image = WebImage.GetImageFromRequest();
            byte[] toPutInDb = image.GetBytes();
            //Klant.Logo = toPutInDb;
            return RedirectToAction("Details", new { id = Klant.KlantId });
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

        [Route("Klant/KlantAccount/Create")]
        [Authorize(Roles = "Klant")]
        public ActionResult Create()
        {

            return View();
        }

        // POST
        [Route("Klant/KlantAccount/Create")]
        [Authorize(Roles = "Klant")]
        [HttpPost]
        public ActionResult Create(Klant Klant, FormCollection collection)
        {
            Klant k = mgr.GetKlant(User.Identity.GetUserName());
            Klant email = mgr.GetKlant(Klant.Email);
            if (email != null)
            {
                //ViewBag.errorMessage = "email moet uniek zijn";
                ModelState.AddModelError("", "Email moet uniek zijn");
                return View("Create");
            }
            Klant naam = mgr.GetKlantByName(Klant.Naam);
            if (naam !=null && naam.IsKlantAccount == false)
            {
                //ViewBag.errorMessage = "naam moet uniek zijn";
                ModelState.AddModelError("", "Er is al reeds een klant met deze naam enkel accounts mogen dezelfde naam hebben");
                return View("Create");
            }
            else
            {
                Klant = mgr.AddKlantAccount(Klant.Naam, Klant.Email, k);
                return RedirectToAction("Details", new { id = Klant.KlantId });
            }

            
        }

        [Route("Klant/KlantAccount/Delete/{id}")]
        [Authorize(Roles = "Klant")]
        public ActionResult Delete(int id)
        {
            Klant Klant = mgr.GetKlant(id);
            return View(Klant);
        }

        // POST
        [HttpPost]
        [Route("Klant/KlantAccount/Delete/{id}")]
        [Authorize(Roles = "Klant")]
        public ActionResult Delete(int id, FormCollection collection)
        {
            List<OVMLijst> ovms = sshmgr.GetLijstAccount(id).ToList();
            if(ovms==null)
            {
                mgr.RemoveKlantAccount(id);
            }
            else
            {
                sshmgr.RemoveLijstenAccount(id);
                mgr.RemoveKlantAccount(id);
            }           
            return RedirectToAction("Index");
        }

        // GET
        [Route("Klant/KlantAccount/Details/{id}")]
        [Authorize(Roles = "Klant")]
        public ActionResult Details(int id)
        {
            Klant Klant = mgr.GetKlant(id);
            return View(Klant);
        }

        // GET
        [Route("Klant/KlantAccount/Edit/{id}")]
        [Authorize(Roles = "Klant")]
        public ActionResult Edit(int id)
        {
            Klant Klant = mgr.GetKlant(id);
            return View(Klant);
        }

        //POST
        [Route("Klant/KlantAccount/Edit/{id}")]
        [Authorize(Roles = "Klant")]
        [HttpPost]
        public ActionResult Edit(Klant Klant)
        {
            if (ModelState.IsValid)
            {
                //Klant k = mgr.GetKlant(User.Identity.GetUserName());
                //Klant email = mgr.GetKlant(Klant.Email);
                //if (email != null)
                //{
                //    //ViewBag.errorMessage = "email moet uniek zijn";
                //    ModelState.AddModelError("", "Email moet uniek zijn");
                //    return View("Edit");
                //}
                //Klant naam = mgr.GetKlantByName(Klant.Naam);
                //if (naam != null && naam.IsKlantAccount == false)
                //{
                //    //ViewBag.errorMessage = "naam moet uniek zijn";
                //    ModelState.AddModelError("", "Er is al reeds een klant met deze naam enkel accounts mogen dezelfde naam hebben");
                //    return View("Edit");
                //}
                Klant.IsKlantAccount = true;
                mgr.ChangeKlant(Klant);
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET
        [Route("Klant/KlantAccount/Block/{id}")]
        [Authorize(Roles = "Klant")]
        public ActionResult Block(int id)
        {
            Klant Klant = mgr.GetKlant(id);
            return View(Klant);
        }

        // POST
        [HttpPost]
        [Authorize(Roles = "Klant")]
        [Route("Klant/KlantAccount/Block/{id}")]
        public ActionResult Block(int id, FormCollection collection)
        {
            mgr.BlockKlantAccount(id);
            return RedirectToAction("Index");
        }

        // GET
        [Route("Klant/KlantAccount/Unblock/{id}")]
        [Authorize(Roles = "Klant")]
        public ActionResult Unblock(int id)
        {
            Klant Klant = mgr.GetKlant(id);
            return View(Klant);
        }

        // POST
        [HttpPost]
        [Authorize(Roles = "Klant")]
        [Route("Klant/KlantAccount/Unblock/{id}")]
        public ActionResult Unblock(int id, FormCollection collection)
        {
            mgr.UnblockKlantAccount(id);
            return RedirectToAction("Index");
        }
    }
    }
