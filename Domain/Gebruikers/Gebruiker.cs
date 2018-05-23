using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain.Gebruikers
{
    public class Gebruiker : IdentityUser
    {
        public int GebruikerId { get; set; }
        public String Naam { get; set; }
        public RolType Rol { get; set; }
        public Boolean Toegestaan { get; set; }
        public Boolean MustChangePassword { get; set; }
        public DateTime LastPasswordChangedDate { get; set; }
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
