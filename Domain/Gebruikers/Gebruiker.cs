using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace Domain.Gebruikers
{
    public class Gebruiker : IdentityUser
    {

        public int GebruikerId { get; set; }
        public String Naam { get; set; }
        //public RolType Rol { get; set; }
        public Boolean Toegestaan { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<Gebruiker> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<Gebruiker> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }


    }
}
