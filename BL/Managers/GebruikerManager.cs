﻿using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using Domain.Gebruikers;
using DAL.Repositories;

namespace BL
{
    public class  GebruikerManager : UserManager<Gebruiker>, IGebruikerManager
    {
        private readonly GebruikerRepository _gebruikerRepository;
        private GebruikerManager(IUserStore<Gebruiker> store) : base(store)
        {
            _gebruikerRepository = (GebruikerRepository)store;
        }
        //Zorgt dat er security in de users zit.
        public static GebruikerManager Create(IDataProtectionProvider provider)
        {
            var manager = new GebruikerManager(new GebruikerRepository());
            Configure(manager, provider);
            return manager;
        }
        //Deze methode haalt alle gebruikers op.
        public List<Gebruiker> GetGebruikers()
        {
            return _gebruikerRepository.ReadGebruikers();
        }
        //Deze methode haalt een gebruiker op aan de hand van zijn username.
        public Gebruiker GetGebruiker(string username)
        {
            return _gebruikerRepository.FindGebruiker(username);
        }
        //Deze methode haalt een gebruiker op aan de hand van het gebruikersid
        public Gebruiker GetGebruiker(int gebruikersid)
        {
            return _gebruikerRepository.FindGebruiker(gebruikersid);
        }
        //Deze methode updatet een gebruiker.
        public void UpdateGebruiker(Gebruiker user)
        {
            _gebruikerRepository.UpdateGebruiker(user);
        }
        //Met deze methode kunnen we onze gebruikermanager configureren.
        //We configureren hier alle voorwaarden en extra services.
        private static void Configure(GebruikerManager manager, IDataProtectionProvider provider)
        {
            manager.UserValidator = new UserValidator<Gebruiker>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<Gebruiker>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<Gebruiker>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            //manager.SmsService = new SmsService();
            if (provider != null)
            {
                manager.UserTokenProvider =
                       new DataProtectorTokenProvider<Gebruiker>(provider.Create("ASP.NET Identity"));
            }
        }
    }
}
