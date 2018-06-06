using DAL.Interfaces;
using DAL.Repositories;
using Domain;
using Domain.Gebruikers;
using System.Collections.Generic;
using System.Linq;

namespace BL
{
    public class KlantManager : IKlantManager
    {
        private readonly IKlantRepository repo;
        private readonly IGebruikerRepository repoUser;
        public KlantManager()
        {
            repo = new KlantRepository();
            repoUser = new GebruikerRepository();
        }
        //Deze methode maakt een klant aan.
        public Klant AddKlant(string naam, string email, string afkorting)
        {
            //hier wordt een nieuwe klant gemaakt.
            Klant k = new Klant()
            {
                Naam = naam,
                Email = email,
                IsGeblokkeerd = false,
                Afkorting=afkorting
            };
            //deze wordt hier aangemaakt in onze KlantRepsoitory.
            Klant created = repo.CreateKlant(k);
            //Er Wordt meteen ook een gebruiker aangemaakt voor deze klant.
            repoUser.CreateGebruiker(email, naam, created.KlantId, RolType.Klant);
            return k;
        }
        //Deze methode updatet een Klant en de aan hem gelinkte Gebruiker.
        public void ChangeKlant(Klant Klant)
        {
            Gebruiker user = repoUser.FindGebruiker(Klant.KlantId);
            
            user.Email = Klant.Email;
            user.Naam = Klant.Naam;
            user.UserName = Klant.Email;
            repoUser.UpdateGebruiker(user);
            repo.UpdateKlant(Klant);
        }
        //Deze methode blokkeert een klant en de aan hem gelinkte gebruiker zodat deze niet meer kan inloggen.
        //Indien het een klant (Admin) is wordt de aan hem gelinkte KlantAccounts ook geblokeerd.
        //Indien het een KlantAccount is wordt enkel deze geblokkerd.
        public void BlockKlant(int id)
        {
            Klant k = GetKlant(id);
            Gebruiker user = repoUser.FindGebruiker(id);
            //Er wordt gecheckt of het een KlantAccount is of niet.
            if (k.IsKlantAccount==false)
            {
                //klant en gebruiker worden geblokeerd.
                user.Toegestaan = false;
                repoUser.UpdateGebruiker(user);
                repo.BlockKlant(id);
                List<Klant> klantenAcc = new List<Klant>();
                //Alle klanten worden opgehaald.
                klantenAcc = GetKlanten().ToList();
                foreach (Klant acc in klantenAcc)
                {
                    //Wordt gecheckt of de klantAccounts van de klant zijn.
                    if (acc.HoofdKlant == k)
                    {
                        //KlantAccount en zijn gebruiker worden geblokkeerd.
                        Gebruiker userAcc = repoUser.FindGebruiker(acc.KlantId);
                        userAcc.Toegestaan = false;
                        repoUser.UpdateGebruiker(userAcc);
                        repo.BlockKlant(acc.KlantId);
                    }
                }
            }
            //Indien KlantAccount is wordt enkel deze geblokkeerd.
            else
            {   
                user.Toegestaan = false;
                repoUser.UpdateGebruiker(user);
                repo.BlockKlant(id);
            }
        }
        //Deze methode deblokkeert een klant en de aan hem gelinkte gebruiker zodat deze terug kan inloggen.
        //Indien het een klant (Admin) is wordt de aan hem gelinkte KlantAccounts ook geblokeerd.
        //Indien het een KlantAccount is wordt enkel deze gedeblokkerd.
        public void UnblockKlant(int id)
        {
            Klant k = GetKlant(id);
            Gebruiker user = repoUser.FindGebruiker(id);
            //Er wordt gecheckt of het een KlantAccount is of niet.
            if (k.IsKlantAccount == false)
            {
                //klant en gebruiker worden geblokeerd.
                user.Toegestaan = true;
                repoUser.UpdateGebruiker(user);
                repo.UnblockKlant(id);
                //Alle klantenAccounts worden opgehaald.
                List<Klant> klantenAcc = new List<Klant>();
                klantenAcc = GetKlantenAccounts(k).ToList();
                foreach (Klant acc in klantenAcc)
                {
                        //KlantAccount en zijn gebruiker worden gedeblokkeerd.
                        Gebruiker userAcc = repoUser.FindGebruiker(acc.KlantId);
                        userAcc.Toegestaan = true;
                        repoUser.UpdateGebruiker(userAcc);
                        repo.UnblockKlant(acc.KlantId);
                    
                }
            }
            //Indien KlantAccount is wordt enkel deze gedeblokkeerd.
            else
            {               
                user.Toegestaan = true;
                repoUser.UpdateGebruiker(user);
                repo.UnblockKlant(id);
            }
        }
        //Deze methode haalt een klant op aan de hand van id.
        public Klant GetKlant(int id)
        {
            return repo.GetKlant(id);
        }
        //Deze methode haalt een klant op aan de hand van de e-mail.
        public Klant GetKlant(string email)
        {
            return repo.GetKlant(email);
        }
        //Deze methode haalt een klant op aan de hand van de naam.
        public Klant GetKlantByName(string naam)
        {
            return repo.GetKlantByName(naam);
        }
        //Deze methode haalt alle klanten op.
        public IEnumerable<Klant> GetKlanten()
        {
            return repo.ReadKlanten();
        }
        //Deze methode haalt alle klanten(admins) op.
        public IEnumerable<Klant> GetHoofdKlanten()
        {
            return repo.ReadHoofdKlanten();
        }
        //Deze methode haalt alle klantAccounts op van een klant.
        public IEnumerable<Klant> GetKlantenAccounts(Klant k)
        {
            List<Klant> klantenAcc = new List<Klant>();
            List<Klant> klantenAccs = new List<Klant>();
            klantenAcc = GetKlanten().ToList();
            foreach (Klant acc in klantenAcc)
            {
                if (acc.HoofdKlant == k)
                {
                    klantenAccs.Add(acc);
                }
            }
            return klantenAccs;
        }
        //Deze methode maakt een klantAccount aan.
        public Klant AddKlantAccount(string naam, string email, Klant h)
        {
            //hier wordt een nieuwe klantAccount gemaakt.
            Klant k = new Klant()
            {
                Naam = naam,
                Email = email,
                IsKlantAccount = true,
                HoofdKlant = h,
                IsGeblokkeerd = false
            };
            //deze wordt hier aangemaakt in onze KlantRepsoitory.
            Klant created = repo.CreateKlant(k);
            //Er Wordt meteen ook een gebruiker aangemaakt voor deze klant.
            repoUser.CreateGebruiker(email, naam, created.KlantId, RolType.KlantAccount);
            return k;
        }
        //Deze methode blokkeert een KlantAccount en de aan hem gelinkte Gebruiker zodat deze niet meer kan inloggen.
        public void BlockKlantAccount(int id)
        {
            Klant k = GetKlant(id);
            Gebruiker user = repoUser.FindGebruiker(id);
            user.Toegestaan = false;
            repoUser.UpdateGebruiker(user);
            repo.BlockKlant(id);
        }
        //Deze methode deblokkeert een KlantAccount en de aan hem gelinkte Gebruiker zodat deze terug kan inloggen.
        public void UnblockKlantAccount(int id)
        {
            Klant k = GetKlant(id);
            Gebruiker user = repoUser.FindGebruiker(id);
            user.Toegestaan = true;
            repoUser.UpdateGebruiker(user);
            repo.UnblockKlant(id);
        }
        //Deze methode verwijdert een klant en de aan hem gelinkte Gebruiker.
        //Indien het een Hoofdklant(KlantAdmin) worden al zijn KlantAccounts en de aan hun gelinkte Gebruikers verwijdert.
        public void RemoveKlant(int id)
        {
            Klant k = GetKlant(id);
            Gebruiker user = repoUser.FindGebruiker(id);
            //Wordt gecheckt of het een KlantAccount is.
            if (k.IsKlantAccount == false)
            {              
                List<Klant> klantenAcc = new List<Klant>();
                klantenAcc = GetKlantenAccounts(k).ToList();
                //Alle KlantAccounts worden opgehaald.
                foreach (Klant acc in klantenAcc)
                {
                        //De KlantAccount en zijn Gebruiker worden verwijderd.
                        Gebruiker userAcc = repoUser.FindGebruiker(acc.KlantId);
                        repo.DeleteKlant(acc);
                        repoUser.DeleteGebruiker(userAcc);
                    
                }
                //De Klant(Admin) en zijn gebruiker worden verwijderd.
                repo.DeleteKlant(k);
                repoUser.DeleteGebruiker(user);
            }
            else
            {   
                //Indien KlantAccount wordt enkel deze en zijn gebruiker verwijderd.
                repo.DeleteKlant(k);
                repoUser.DeleteGebruiker(user);
            }
        }
        //Deze methode verwijdert een KlantAccount en zijn gebruiker.
        public void RemoveKlantAccount(int id)
        {
            Klant k = GetKlant(id);
            Gebruiker user = repoUser.FindGebruiker(id);
            repo.DeleteKlant(k);
            repoUser.DeleteGebruiker(user);
        }
        //Haalt een Klant(Admin) op.
        public Klant GetHoofdKlant(int klantId)
        {
            return repo.ReadHoofdKlant(klantId);
        }
    }
}
