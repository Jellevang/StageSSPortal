using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Domain.Gebruikers;
using DAL.EF;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class GebruikerRepository : UserStore<Gebruiker>, IGebruikerRepository // Bij ophalen etc checken op actief !!
    {
        private readonly StageSSPortalDbContext ctx;
        private Gebruiker user;
        private UserManager<Gebruiker> userManager;

        public GebruikerRepository() : base(new StageSSPortalDbContext())
        {
            ctx = (StageSSPortalDbContext)this.Context;
            userManager = new UserManager<Gebruiker>(new GebruikerRepository(ctx));
        }
        /*public GebruikerRepository(UnitOfWork uow) : base(uow.Context)
        {
            ctx = uow.Context;
        }*/
        public GebruikerRepository(StageSSPortalDbContext context) : base(context)
        {
            ctx = context;
        }

        public List<Gebruiker> ReadGebruikers()
        {
            return ctx.Users.ToList();
        }

        public Gebruiker FindGebruiker(int gebruikerId)
        {
            List<Gebruiker> users = ReadGebruikers();
            Gebruiker user = users.Find(x => x.GebruikerId == gebruikerId);
            return user;
        }

        public void UpdateGebruiker(Gebruiker user)
        {
            Gebruiker aanTePassenGebruiker = ctx.Users.Find(user.Id);
            ctx.Entry(aanTePassenGebruiker).CurrentValues.SetValues(user);
            ctx.Entry(aanTePassenGebruiker).State = System.Data.Entity.EntityState.Modified;
            ctx.SaveChanges();
        }

        public void DeleteGebruiker(Gebruiker user)
        {
            var logins = user.Logins;
            var rolesForUser = userManager.GetRoles(user.Id);

            foreach (var login in logins.ToList())
            {
                userManager.RemoveLogin(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
            }

            if (rolesForUser.Any())
            {
                foreach (var item in rolesForUser.ToList())
                {
                    userManager.RemoveFromRole(user.Id, item);
                }
            }

            userManager.Delete(user);
        }

        public Gebruiker CreateGebruiker(string email, string naam, int gebruikerid, RolType rol)
        {

            IdentityRole userRole = null;
            if (rol.Equals(1))
            {
                if (!ctx.Roles.Any(r => r.Name == "Admin"))
                {
                    userRole = new IdentityRole("Admin");
                    ctx.Roles.Add(userRole);
                }
                else
                {
                    userRole = ctx.Roles.FirstOrDefault(r => r.Name == "Admin");
                }
            }
            /*if (rol.Equals(3))
            {
                if (!ctx.Roles.Any(r => r.Name == "SuperAdmin"))
                {
                    userRole = new IdentityRole("SuperAdmin");
                    ctx.Roles.Add(userRole);
                }
                else
                {
                    userRole = ctx.Roles.FirstOrDefault(r => r.Name == "SuperAdmin");
                }
            }*/
            else
            {


                if (!ctx.Roles.Any(r => r.Name == "Klant"))
                {
                    userRole = new IdentityRole("Klant");
                    ctx.Roles.Add(userRole);
                }
                else
                {
                    userRole = ctx.Roles.FirstOrDefault(r => r.Name == "Klant");
                }
            }


            if (!ctx.Users.Any(u => u.Email == email))
            {
                var hasher = new PasswordHasher();

                switch (rol)
                {
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
                            SecurityStamp = Guid.NewGuid().ToString()
                        };
                        break;
                    /*case RolType.SuperAdmin:
                        user = new Gebruiker()
                        {
                            Email = email,
                            UserName = email,
                            Rol = rol,
                            GebruikerId = 0,
                            Naam = naam,
                            EmailConfirmed = true,
                            Toegestaan = true,
                            SecurityStamp = Guid.NewGuid().ToString()
                        };
                        break;*/
                    default:
                        user = new Gebruiker()
                        {
                            Email = email,
                            UserName = email,
                            Rol = rol,
                            GebruikerId = gebruikerid,
                            Naam = naam,
                            EmailConfirmed = true,
                            Toegestaan = true,
                            SecurityStamp = Guid.NewGuid().ToString()
                        };
                        break;
                }

                new UserManager<Gebruiker>(new GebruikerRepository(ctx)).Create(user, "Default123$");
                //context.Gebruikers.Add(user);
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
