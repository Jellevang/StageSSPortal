using System.Data.Entity;
using System;
using System.Collections.Generic;
using Domain;
using Domain.Gebruikers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using DAL.Repositories;
using System.Linq;

namespace DAL.EF
{
    internal class StageSSPortalDbInitializer : DropCreateDatabaseIfModelChanges<StageSSPortalDbContext>
    {
        
            protected override void Seed(StageSSPortalDbContext context)
            {
                List<String> emails = new List<string>();
                List<String> namen = new List<string>();
                List<Klant> klant = new List<Klant>();
                
                
                
            
                #region Klanten
                Klant sam = new Klant()
                {
                    Naam = "Sam Heirstrate",
                    Email = "sam.heirstrate@stage.be",
                    IsGeblokkeerd = false
                };

                emails.Add(sam.Email);
                namen.Add(sam.Naam);

                Klant jelle = new Klant()
                {
                    Naam = "Jelle van Ginderen",
                    Email = "jelle.van.ginderen@stage.be",
                    IsGeblokkeerd = false

                };
                emails.Add(jelle.Email);
                namen.Add(jelle.Naam);

                

                context.Klanten.Add(sam);
                context.Klanten.Add(jelle);

                #endregion
                
                
                

              

                //Gebruikers partij en admin createn

                IdentityRole userRole = null;

                #region Admin
                if (!context.Roles.Any(r => r.Name == "Admin"))
                {
                    userRole = new IdentityRole("Admin");
                    context.Roles.Add(userRole);
                }
                else
                {
                    userRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
                }

                if (!context.Users.Any(u => u.Email == "Admin@stage.be"))
                {
                    var hasher = new PasswordHasher();

                    var user = new Gebruiker()
                    {
                        Email = "Admin@stage.be",
                        UserName = "Admin@stage.be",
                        Rol = RolType.Admin,
                        GebruikerId = 10,
                        Naam = "Alfons",
                        EmailConfirmed = true,
                        Toegestaan = true,
                        SecurityStamp = Guid.NewGuid().ToString()

                    };

                    new UserManager<Gebruiker>(new GebruikerRepository(context)).Create(user, "Default123$");
                    //context.Gebruikers.Add(user);
                    if (userRole != null)
                    {
                        user.Roles.Add(new IdentityUserRole { RoleId = userRole.Id, UserId = user.Id });
                    }

                    context.Admins.Add(new Admin()
                    {
                        Email = user.Email,
                        naam = user.Naam,
                        
                    });
                }
                context.SaveChanges();
                #endregion
                for (int i = 0; i < emails.Count; i++)
                {
                    #region Klant
                    if (!context.Roles.Any(r => r.Name == "Klant"))
                    {
                        userRole = new IdentityRole("Klant");
                        context.Roles.Add(userRole);
                    }
                    else
                    {
                        userRole = context.Roles.FirstOrDefault(r => r.Name == "Klant");
                    }

                    if (!context.Users.Any(u => u.Email == "Klant@stage.be"))
                    {
                        var hasher = new PasswordHasher();

                        var user = new Gebruiker()
                        {
                            Email = emails[i],
                            UserName = emails[i],
                            Rol = RolType.Klant,
                            GebruikerId = context.Klanten.Single(p => p.Email == emails[i]).KlantId,
                            Naam = namen[i],
                            EmailConfirmed = true,
                            Toegestaan = true,
                            SecurityStamp = Guid.NewGuid().ToString()

                        };
                        new UserManager<Gebruiker>(new GebruikerRepository(context)).Create(user, "Default123$");
                        

                        if (userRole != null)
                        {
                            user.Roles.Add(new IdentityUserRole { RoleId = userRole.Id, UserId = user.Id });
                        }

                    }
                    context.SaveChanges();
                    #endregion

                    #region SuperAdmin 
                    /*
                    if (!context.Roles.Any(r => r.Name == "SuperAdmin"))
                    {
                        userRole = new IdentityRole("SuperAdmin");
                        context.Roles.Add(userRole);
                    }
                    else
                    {
                        userRole = context.Roles.FirstOrDefault(r => r.Name == "SuperAdmin");
                    }

                    if (!context.Users.Any(u => u.Email == "SuperAdmin@vkt.be"))
                    {
                        var hasher = new PasswordHasher();

                        var user = new Gebruiker()
                        {
                            Email = "SuperAdmin@vkt.be",
                            UserName = "SuperAdmin@vkt.be",
                            Rol = RolType.SuperAdmin,
                            GebruikerId = 11,
                            Naam = "Sam",
                            EmailConfirmed = true,
                            Toegestaan = true,
                            SecurityStamp = Guid.NewGuid().ToString()

                        };

                        new UserManager<Gebruiker>(new GebruikerRepository(context)).Create(user, "Default123$");
                        //context.Gebruikers.Add(user);
                        if (userRole != null)
                        {
                            user.Roles.Add(new IdentityUserRole { RoleId = userRole.Id, UserId = user.Id });
                        }

                        context.Admins.Add(new Admin()
                        {
                            Email = user.Email,
                            naam = user.Naam,
                            IsSuperAdmin = true
                        });
                    }
                    context.SaveChanges();
                    */
                    #endregion

                }


            }
        }
    }
