using BL;
using Domain;
using StageSSPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace StageSSPortal.Controllers.api
{
    public class ManageController : ApiController
    {
        AdminManager adm = new AdminManager();

        [HttpPost]
        [Route("api/manage/ChangeOvmPasswd/{newpasswd}/{oldpasswd}")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult ChangeOvmPasswd(string newpasswd, string oldpasswd)
        {
            Admin a = adm.GetAdmin();
            if (adm.GetPasswd(a) == oldpasswd)
            {
                adm.UpdatePasswd(newpasswd, a);
            }
            else
            {
                return Ok(false);

            }
            return Ok(true);

        }
    }
}
