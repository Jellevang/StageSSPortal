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
        public Klant CreateKlant(Klant Klant)
        {
            ctx.Klanten.Add(Klant);
            ctx.SaveChanges();
            return Klant;
        }
        public void BlockKlant(int id)
        {
            Klant k = ctx.Klanten.Find(id);
            k.IsGeblokkeerd = true;
            ctx.SaveChanges();
        }
        public void UnblockKlant(int id)
        {
            Klant k = ctx.Klanten.Find(id);
            k.IsGeblokkeerd = false;
            ctx.SaveChanges();
        }
        public Klant GetKlant(int id)
        {
            Klant Klant = ctx.Klanten.Where(k => k.KlantId == id).FirstOrDefault();
            return Klant;
        }
        public Klant GetKlant(string email)
        {
            Klant Klant = ctx.Klanten.Where(k => k.Email.Equals(email)).FirstOrDefault();
            return Klant;
        }
        public Klant GetKlantByName(string naam)
        {
            Klant Klant = ctx.Klanten.Where(k => k.Naam.Equals(naam)).FirstOrDefault();
            return Klant;
        }
        public IEnumerable<Klant> ReadKlanten()
        {
            return ctx.Klanten.AsEnumerable();
        }
        public void UpdateKlant(Klant Klant)
        {
            var test=ctx.Klanten.Where(x => x.KlantId == Klant.KlantId).FirstOrDefault();
            test.Naam = Klant.Naam;
            test.Email = Klant.Email;
            test.Afkorting = Klant.Afkorting;
           ctx.Entry(test).State = EntityState.Modified;
            ctx.SaveChanges();
        }
        public void DeleteKlant(Klant k)
        {
            ctx.Klanten.Remove(k);
            ctx.SaveChanges();
        }
        public IEnumerable<Klant> ReadHoofdKlanten()
        {
            return ctx.Klanten.Where(k => k.IsKlantAccount ==false);
        }
        public Klant ReadHoofdKlant(int klantId)
        {
            Klant temp=ctx.Klanten.Where(k=>k.KlantId==klantId).FirstOrDefault();
            Klant tempHoofd = ctx.Klanten.Where(k => k.KlantId == temp.HoofdKlant.KlantId).FirstOrDefault();
            return tempHoofd;
        }
        public IEnumerable<Klant> ReadKlantenAccounts(Klant h)
        {
            return ctx.Klanten.Where(k => k.HoofdKlant == h);
        }
    }
}
