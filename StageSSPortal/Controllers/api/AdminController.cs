using BL;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web.Http;

namespace StageSSPortal.Controllers.api
{
    public class AdminController : ApiController
    {
        AdminManager mgr = new AdminManager();




        [HttpGet]
        [Route("api/SSH/checkPasswd")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult checkPasswd()
        {
            Admin admin = mgr.GetAdmin();
            if (admin.OvmPassword == "" || admin.OvmPassword == null)
            {
                return Ok(false);
            }
            else return Ok(true);

        }

        [HttpPost]
        [Route("api/SSH/InsertPasswd/{passwd}")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult InsertPasswd(string passwd)
        {
            Admin admin = mgr.GetAdmin();
            mgr.UpdatePasswd(passwd, admin);
            return Ok(true);
        }
    }
}