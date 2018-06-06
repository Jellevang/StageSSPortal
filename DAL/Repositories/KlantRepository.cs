using DAL.EF;
using DAL.Interfaces;
using Domain;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Repositories
{
    public class KlantRepository : IKlantRepository
    {
        private readonly StageSSPortalDbContext ctx;
        public KlantRepository()
        {
            ctx = new StageSSPortalDbContext();
        }
        //Hier wordt er een Klant aangemaakt in de databank.
        public Klant CreateKlant(Klant Klant)
        {
            ctx.Klanten.Add(Klant);
            ctx.SaveChanges();
            return Klant;
        }
        //Deze methode blokkeert een klant op basis van de id.
        public void BlockKlant(int id)
        {
            Klant k = ctx.Klanten.Find(id);
            k.IsGeblokkeerd = true;
            ctx.SaveChanges();
        }
        //Deze methode deblokkeert een klant op basis van de id.
        public void UnblockKlant(int id)
        {
            Klant k = ctx.Klanten.Find(id);
            k.IsGeblokkeerd = false;
            ctx.SaveChanges();
        }
        //Deze methode haalt  een klant op op basis van de id.
        public Klant GetKlant(int id)
        {
            Klant Klant = ctx.Klanten.Where(k => k.KlantId == id).FirstOrDefault();
            return Klant;
        }
        //Deze methode haalt  een klant op op basis van de e-mail.
        public Klant GetKlant(string email)
        {
            Klant Klant = ctx.Klanten.Where(k => k.Email.Equals(email)).FirstOrDefault();
            return Klant;
        }
        //Deze methode haalt  een klant op op basis van de naam.
        public Klant GetKlantByName(string naam)
        {
            Klant Klant = ctx.Klanten.Where(k => k.Naam.Equals(naam)).FirstOrDefault();
            return Klant;
        }
        //Deze methode haalt alle klanten op.
        public IEnumerable<Klant> ReadKlanten()
        {
            return ctx.Klanten.AsEnumerable();
        }
        //Deze methode updatet de naam, e-mail en afkorting van een klant.
        //Hier kunnen we meerdere attributen aan toevoegen indien dit gewenst wordt.
        public void UpdateKlant(Klant Klant)
        {
            var test=ctx.Klanten.Where(x => x.KlantId == Klant.KlantId).FirstOrDefault();
            test.Naam = Klant.Naam;
            test.Email = Klant.Email;
            test.Afkorting = Klant.Afkorting;
           ctx.Entry(test).State = EntityState.Modified;
            ctx.SaveChanges();
        }
        //Deze methode verwijdert een klant.
        public void DeleteKlant(Klant k)
        {
            ctx.Klanten.Remove(k);
            ctx.SaveChanges();
        }
        //Deze methode geeft alle hoofdklanten terug.
        //HoofdKlanten zijn Klant Admins. Deze kunnen medewerkers(KlantAccount) aanmaken. Ze hebben als Rol Klant(zie gebruiker).
        public IEnumerable<Klant> ReadHoofdKlanten()
        {
            return ctx.Klanten.Where(k => k.IsKlantAccount ==false);
        }
        //Deze methode geeft een hoofdklant terug aan de hand van een id.
        public Klant ReadHoofdKlant(int klantId)
        {
            Klant temp=ctx.Klanten.Where(k=>k.KlantId==klantId).FirstOrDefault();
            Klant tempHoofd = ctx.Klanten.Where(k => k.KlantId == temp.HoofdKlant.KlantId).FirstOrDefault();
            return tempHoofd;
        }
        //Deze methode geeft alle hoofdklanten terug aan de hand van hun HoofdKlant.
        //KlantAccounts zijn Medewerkers van een KlantAdmin(Klant). Ze hebben als Rol KlantAccount(zie gebruiker).
        public IEnumerable<Klant> ReadKlantenAccounts(Klant h)
        {
            return ctx.Klanten.Where(k => k.HoofdKlant == h);
        }
    }
}
