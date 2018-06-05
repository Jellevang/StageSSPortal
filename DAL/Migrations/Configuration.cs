namespace DAL.Migrations
{
    using System.Data.Entity;
    using System;
    using System.Collections.Generic;
    using Domain;
    using Domain.Gebruikers;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using DAL.Repositories;
    using System.Linq;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<DAL.EF.StageSSPortalDbContext>
    {
        //Migrations configuratie
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "DAL.EF.StageSSPortalDbContext";
        }
        //seed methode om testdata in de databank te steken 
        //Let op niet Gebruiken om te deployen
        protected override void Seed(DAL.EF.StageSSPortalDbContext context)
        {
            List<String> emails = new List<string>();
            List<String> namen = new List<string>();
            List<Klant> klanten = new List<Klant>();
            List<Klant> klantenAccounts = new List<Klant>();
            //List<KlantAccount> klantAccounts = new List<KlantAccount>();

            #region Klanten
            Klant sam = new Klant()
            {
                Naam = "Sam Heirstrate",
                Email = "sam.heirstrate@stage.be",
                IsKlantAccount = false,
                IsGeblokkeerd = false
            };

            klanten.Add(sam);
            emails.Add(sam.Email);
            namen.Add(sam.Naam);

            Klant jelle = new Klant()
            {
                Naam = "Jelle van Ginderen",
                Email = "jelle.van.ginderen@stage.be",
                IsKlantAccount = false,
                IsGeblokkeerd = false

            };

            klanten.Add(jelle);
            emails.Add(jelle.Email);
            namen.Add(jelle.Naam);

            Klant monin = new Klant()
            {
                Naam = "monin-it",
                Email = "monin@stage.be",
                IsKlantAccount = false,
                IsGeblokkeerd = false
            };


            klanten.Add(monin);
            emails.Add(monin.Email);
            namen.Add(monin.Naam);


            context.Klanten.Add(sam);
            context.Klanten.Add(jelle);
            context.Klanten.Add(monin);

            #endregion
            #region MoninAccounts
            Klant gert = new Klant()
            {
                Naam = "Gert de Neve",
                Email = "gert@stage.be",
                IsKlantAccount = true,
                HoofdKlant = monin,
                IsGeblokkeerd = false
            };
            klantenAccounts.Add(gert);
            Klant davy = new Klant()
            {
                Naam = "Davy van Mol",
                Email = "davy@stage.be",
                IsKlantAccount = true,
                HoofdKlant = monin,
                IsGeblokkeerd = false
            };
            klantenAccounts.Add(davy);
            Klant dries = new Klant()
            {
                Naam = "Dries Moelans",
                Email = "dries@stage.be",
                IsKlantAccount = true,
                HoofdKlant = monin,
                IsGeblokkeerd = false
            };
            klantenAccounts.Add(dries);
            context.Klanten.Add(gert);
            context.Klanten.Add(davy);
            context.Klanten.Add(dries);
            #endregion
            #region KlantAccounts
            //KlantAccount gert = new KlantAccount()
            //{
            //    Naam = "Gert de Neve",
            //    KlantId = monin.KlantId,
            //    Email = "gert@stage.be",
            //    IsGeblokkeerd = false

            //};
            //klantAccounts.Add(gert);
            //KlantAccount davy = new KlantAccount()
            //{
            //    Naam = "Davy van Mol",
            //    KlantId = monin.KlantId,
            //    Email = "davy@stage.be",
            //    IsGeblokkeerd = false

            //};
            //klantAccounts.Add(davy);
            //KlantAccount dries = new KlantAccount()
            //{
            //    Naam = "Dries Moelans",
            //    KlantId = monin.KlantId,
            //    Email = "dries@stage.be",
            //    IsGeblokkeerd = false

            //};
            //klantAccounts.Add(dries);
            //context.KlantAccounten.Add(gert);
            //context.KlantAccounten.Add(davy);
            //context.KlantAccounten.Add(dries);
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
                    GebruikerId = 10000000,
                    Naam = "Alfons",
                    EmailConfirmed = true,
                    Toegestaan = true,
                    MustChangePassword = false,
                    LastPasswordChangedDate = DateTime.Now,
                    SecurityStamp = Guid.NewGuid().ToString()

                };

                new UserManager<Gebruiker>(new GebruikerRepository(context)).Create(user, "stage123");
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

            for (int i = 0; i < klanten.Count; i++)
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
                        Email = klanten[i].Email,
                        UserName = klanten[i].Email,
                        Rol = RolType.Klant,
                        //GebruikerId = context.Klanten.Single(p => p.Email == emails[i]).KlantId,
                        GebruikerId = klanten[i].KlantId,
                        Naam = klanten[i].Naam,
                        EmailConfirmed = true,
                        Toegestaan = true,
                        MustChangePassword = false,
                        LastPasswordChangedDate = DateTime.Now,
                        SecurityStamp = Guid.NewGuid().ToString()

                    };
                    new UserManager<Gebruiker>(new GebruikerRepository(context)).Create(user, "stage123");


                    if (userRole != null)
                    {
                        user.Roles.Add(new IdentityUserRole { RoleId = userRole.Id, UserId = user.Id });
                    }

                }
                context.SaveChanges();
                #endregion

            }
            for (int i = 0; i < klantenAccounts.Count; i++)
            {
                #region Klant
                if (!context.Roles.Any(r => r.Name == "KlantAccount"))
                {
                    userRole = new IdentityRole("KlantAccount");
                    context.Roles.Add(userRole);
                }
                else
                {
                    userRole = context.Roles.FirstOrDefault(r => r.Name == "KlantAccount");
                }

                if (!context.Users.Any(u => u.Email == "KlantAccount@stage.be"))
                {
                    var hasher = new PasswordHasher();

                    var user = new Gebruiker()
                    {
                        Email = klantenAccounts[i].Email,
                        UserName = klantenAccounts[i].Email,
                        Rol = RolType.KlantAccount,
                        //GebruikerId = context.Klanten.Single(p => p.Email == emails[i]).KlantId,
                        GebruikerId = klantenAccounts[i].KlantId,
                        Naam = klantenAccounts[i].Naam,
                        EmailConfirmed = true,
                        Toegestaan = true,
                        MustChangePassword = false,
                        LastPasswordChangedDate = DateTime.Now,
                        SecurityStamp = Guid.NewGuid().ToString()

                    };
                    new UserManager<Gebruiker>(new GebruikerRepository(context)).Create(user, "stage123");


                    if (userRole != null)
                    {
                        user.Roles.Add(new IdentityUserRole { RoleId = userRole.Id, UserId = user.Id });
                    }

                }
                context.SaveChanges();
                #endregion
            }
        }
    }
}
