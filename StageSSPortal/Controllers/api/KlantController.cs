using BL;
using DAL.EF;
using Domain;
using Domain.Gebruikers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
        private readonly IKlantManager mgr = new KlantManager();
        private readonly UserManager<Gebruiker> userManager = new UserManager<Gebruiker>(new UserStore<Gebruiker>(new StageSSPortalDbContext()));

        [HttpGet]
        [Route("api/klant/getHoofdAcc/{id}")]
      //  [Authorize(Roles = "Admin")]
        public IHttpActionResult GetHoofdAcc(int klantId)
        {
            try
            {
                Klant temp = mgr.GetHoofdKlant(klantId);
                return Ok(temp.KlantId);
            }
            catch
            {
                return Ok(0);
            }
            
        }


    }
}
