using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Domain.Gebruikers;
using DAL.EF;
using DAL.Interfaces;

namespace DAL.Repositories
{
    //We erven hier ook over van UserStore<Gebruiker> omdat we met identity werken en onze gebruiker een Identity user is.
    public class GebruikerRepository : UserStore<Gebruiker>, IGebruikerRepository 
    {
        private readonly StageSSPortalDbContext ctx;
        private Gebruiker user;
        private UserManager<Gebruiker> userManager;
        public GebruikerRepository() : base(new StageSSPortalDbContext())
        {
            ctx = (StageSSPortalDbContext)this.Context;
            userManager = new UserManager<Gebruiker>(new GebruikerRepository(ctx));
        }
        public GebruikerRepository(StageSSPortalDbContext context) : base(context)
        {
            ctx = context;
        }
        //Geeft alle Gebruikers in een List<Gebruiker> terug.
        public List<Gebruiker> ReadGebruikers()
        {
            return ctx.Users.ToList();
        }
        //Zoek een gebruiker en geef deze terug aan de hand van een gebruikerId.
        public Gebruiker FindGebruiker(int gebruikerId)
        {
            List<Gebruiker> users = ReadGebruikers();
            Gebruiker user = users.Find(x => x.GebruikerId == gebruikerId);
            return user;
        }
        //Zoek een gebruiker en geef deze terug aan de hand van een gebruikersnaam.
        //Username is uniek omdat we deze hetezelfde maken als het e-mail van de gebruiker.
        public Gebruiker FindGebruiker(string username)
        {
            List<Gebruiker> users = ReadGebruikers();
            Gebruiker user = users.Find(x => x.UserName.Equals(username));
            return user;
        }
        //Verander attributen van een gebruiker.
        public void UpdateGebruiker(Gebruiker user)
        {
            Gebruiker aanTePassenGebruiker = ctx.Users.Find(user.Id);
            ctx.Entry(aanTePassenGebruiker).CurrentValues.SetValues(user);
            ctx.Entry(aanTePassenGebruiker).State = System.Data.Entity.EntityState.Modified;
            ctx.SaveChanges();
        }
        //Verwijdert een gebruiker en al zijn verwijzingen in andere tables zoals, in identityuserrole, identityuserlogin en identityuserclaim.
        public void DeleteGebruiker(Gebruiker user)
        {
            var logins = user.Logins;
            var rolesForUser = userManager.GetRoles(user.Id);
            //Hier verwijdert hij de logins.
            foreach (var login in logins.ToList())
            {
                userManager.RemoveLogin(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
            }
            //Hier verwijdert hij de identityuserrole
            if (rolesForUser.Any())
            {
                foreach (var item in rolesForUser.ToList())
                {
                    userManager.RemoveFromRoleAsync(user.Id, item);
                }
            }

            userManager.Delete(user);
        }
        //Hier wordt een gebruiker aangemaakt met de juiste Rol
        //e-mail, naam, gebruikerid en rol worden meegegeven. (gebruikerid verwijst naar een klantid of een admin id).
        public Gebruiker CreateGebruiker(string email, string naam, int gebruikerid, RolType rol)
        {
            //als de rol Admin is komt hij in deze lus
            IdentityRole userRole = null;
            if (rol.Equals(RolType.Admin))
            {
                //indien de Rol Admin nog niet bestaat wordt deze aangemaakt.
                if (!ctx.Roles.Any(r => r.Name == "Admin"))
                {
                    userRole = new IdentityRole("Admin");
                    ctx.Roles.Add(userRole);
                }
                //hier wordt deze opgehaald en in userRole gestoken.
                else
                {
                    userRole = ctx.Roles.FirstOrDefault(r => r.Name == "Admin");
                }
            }
            //Als de rol KlantAccount is komt hij in deze lus terecht.
            if (rol.Equals(RolType.KlantAccount))
            {
                //indien de Rol KlantAccount nog niet bestaat wordt deze aangemaakt.
                if (!ctx.Roles.Any(r => r.Name == "KlantAccount"))
                {
                    userRole = new IdentityRole("KlantAccount");
                    ctx.Roles.Add(userRole);
                }
                //hier wordt deze opgehaald en in userRole gestoken.
                else
                {
                    userRole = ctx.Roles.FirstOrDefault(r => r.Name == "KlantAccount");
                }
                
            }
            //Als de rol Klant is komt hij in deze lus terecht.
            else
            {
                //indien de Rol Klant nog niet bestaat wordt deze aangemaakt.
                if (!ctx.Roles.Any(r => r.Name == "Klant"))
                {
                    userRole = new IdentityRole("Klant");
                    ctx.Roles.Add(userRole);
                }
                //hier wordt deze opgehaald en in userRole gestoken.
                else
                {
                    userRole = ctx.Roles.FirstOrDefault(r => r.Name == "Klant");
                }
            }
            //als de gegeven email nog niet aan een Gebruiker is gelinkt gaan we verder in deze lus.
            if (!ctx.Users.Any(u => u.Email == email))
            {
                //hier wordt een PasswordHasher aangemaakt.
                var hasher = new PasswordHasher();
                switch (rol)
                {
                    //Hier wordt een Admin Gebruiker aangemaakt.
                    case RolType.Admin:
                        user = new Gebruiker()
                        {
                            Email = email,
                            UserName = email,
                            Rol = rol,
                            GebruikerId = 0,
                            Naam = naam,
                            EmailConfirmed = true,
                            Toegestaan = true,
                            MustChangePassword=true,
                            LastPasswordChangedDate = DateTime.Now,
                            SecurityStamp = Guid.NewGuid().ToString()
                        };
                        break;
                    //Hier wordt een KlantAccount Gebruiker aangemaakt.
                    case RolType.KlantAccount:
                        user = new Gebruiker()
                        {
                            Email = email,
                            UserName = email,
                            Rol = rol,
                            GebruikerId = gebruikerid,
                            Naam = naam,
                            EmailConfirmed = true,
                            Toegestaan = true,
                            MustChangePassword = true,
                            LastPasswordChangedDate = DateTime.Now,
                            SecurityStamp = Guid.NewGuid().ToString()
                        };
                        break;
                    default:
                        //Hier wordt een Klant Gebruiker aangemaakt.
                        user = new Gebruiker()
                        {
                            Email = email,
                            UserName = email,
                            Rol = rol,
                            GebruikerId = gebruikerid,
                            Naam = naam,
                            EmailConfirmed = true,
                            Toegestaan = true,
                            MustChangePassword = true,
                            LastPasswordChangedDate = DateTime.Now,
                            SecurityStamp = Guid.NewGuid().ToString()
                        };
                        break;
                }
                //Hier wordt een default passwoord toegekend aan de gebruiker.
                //Dit zal hij moeten aanpassen als hij voor de eerste keer inlogd, zie verder in Accountcontroller.
                new UserManager<Gebruiker>(new GebruikerRepository(ctx)).Create(user, "Default123$");
                if (userRole != null)
                {
                    user.Roles.Add(new IdentityUserRole { RoleId = userRole.Id, UserId = user.Id });
                }
            }
            ctx.SaveChanges();
            return user;
        }
    }
}
