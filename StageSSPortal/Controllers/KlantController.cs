﻿using System;
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
    public class KlantController : Controller
    {
        private readonly IKlantManager mgr = new KlantManager();
        private readonly UserManager<Gebruiker> userManager = new UserManager<Gebruiker>(new UserStore<Gebruiker>(new StageSSPortalDbContext()));
        public virtual ActionResult Klanten()
        {
            return View();
        }
        [Route("Klant/Home")]
        [Authorize(Roles = "Klant")]
        public ActionResult Home()
        {
            return View();
        }
        // GET: Klant
        [Route("Admin/Klant")]
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            IEnumerable<Klant> Klanten = mgr.GetKlanten();
            return View(Klanten);
        }

        // GET: Klant-informatie
        [Route("Klant/Profiel/")]
        [Authorize(Roles = "Klant")]
        public ActionResult Profiel()
        {
            var user = userManager.FindById(User.Identity.GetUserId());
            ViewBag.Id = user.GebruikerId;
            return View();
        }

        

        [Route("Klant/Profiel/Create")]
        [Authorize(Roles = "Klant")]
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

        [Route("Admin/Klant/Create")]
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {

            return View();
        }

        // POST
        [Route("Admin/Klant/Create")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(Klant Klant, FormCollection collection)
        {
            Klant = mgr.AddKlant(Klant.Naam, Klant.Email);
            return RedirectToAction("Details", new { id = Klant.KlantId });
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