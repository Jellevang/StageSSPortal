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
        public Klant AddKlant(string naam, string email)
        {
            Klant k = new Klant()
            {
                Naam = naam,
                Email = email,
                Tag = naam.Substring(0,3),
                IsGeblokkeerd = false
            };
            Klant created = repo.CreateKlant(k);
            created.Tag = created.Tag + created.KlantId.ToString();
            repo.UpdateKlant(created);
            repoUser.CreateGebruiker(email, naam, created.KlantId, RolType.Klant);
            return k;
        }
        public void ChangeKlantprofiel(Klant k)
        {
            Klant Klant = repo.GetKlant(k.KlantId);
            Klant.Naam = k.Naam;
            repo.UpdateKlant(k);
        }
        public void ChangeKlant(Klant Klant)
        {
            Gebruiker user = repoUser.FindGebruiker(Klant.KlantId);
            user.Email = Klant.Email;
            user.Naam = Klant.Naam;
            user.UserName = Klant.Email;
            repoUser.UpdateGebruiker(user);
            repo.UpdateKlant(Klant);
        }
        public void BlockKlant(int id)
        {
            Klant k = GetKlant(id);
            Gebruiker user = repoUser.FindGebruiker(id);
            if (k.IsKlantAccount==false)
            {
                user.Toegestaan = false;
                repoUser.UpdateGebruiker(user);
                repo.BlockKlant(id);
                List<Klant> klantenAcc = new List<Klant>();
                klantenAcc = GetKlanten().ToList();
                foreach (Klant acc in klantenAcc)
                {
                    if (acc.HoofdKlant == k)
                    {
                        Gebruiker userAcc = repoUser.FindGebruiker(acc.KlantId);
                        userAcc.Toegestaan = false;
                        repoUser.UpdateGebruiker(userAcc);
                        repo.BlockKlant(acc.KlantId);
                    }
                }
            }
            else
            {
                user.Toegestaan = false;
                repoUser.UpdateGebruiker(user);
                repo.BlockKlant(id);
            }
        }
        public void UnblockKlant(int id)
        {
            Klant k = GetKlant(id);
            Gebruiker user = repoUser.FindGebruiker(id);
            if (k.IsKlantAccount == false)
            {
                user.Toegestaan = true;
                repoUser.UpdateGebruiker(user);
                repo.UnblockKlant(id);
                List<Klant> klantenAcc = new List<Klant>();
                klantenAcc = GetKlantenAccounts(k).ToList();
                foreach (Klant acc in klantenAcc)
                {
                    
                        Gebruiker userAcc = repoUser.FindGebruiker(acc.KlantId);
                        userAcc.Toegestaan = true;
                        repoUser.UpdateGebruiker(userAcc);
                        repo.UnblockKlant(acc.KlantId);
                    
                }
                //klantenAcc = GetKlanten().ToList();
                //foreach (Klant acc in klantenAcc)
                //{
                //    if (acc.HoofdKlant == k)
                //    {
                //        Gebruiker userAcc = repoUser.FindGebruiker(acc.KlantId);
                //        userAcc.Toegestaan = true;
                //        repoUser.UpdateGebruiker(userAcc);
                //        repo.UnblockKlant(acc.KlantId);
                //    }
                //}
            }
            else
            {               
                user.Toegestaan = true;
                repoUser.UpdateGebruiker(user);
                repo.UnblockKlant(id);
            }
        }
        public Klant GetKlant(int id)
        {
            return repo.GetKlant(id);
        }
        public Klant GetKlant(string email)
        {
            return repo.GetKlant(email);
        }
        public Klant GetKlantByName(string naam)
        {
            return repo.GetKlantByName(naam);
        }
        public IEnumerable<Klant> GetKlanten()
        {
            return repo.ReadKlanten();
        }
        public IEnumerable<Klant> GetHoofdKlanten()
        {
            return repo.ReadHoofdKlanten();
        }
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
        public Klant AddKlantAccount(string naam, string email, Klant h)
        {
            Klant k = new Klant()
            {
                Naam = naam,
                Email = email,
                Tag = naam.Substring(0, 3),
                IsKlantAccount = true,
                HoofdKlant = h,
                IsGeblokkeerd = false
            };
            Klant created = repo.CreateKlant(k);
            created.Tag = created.Tag + created.KlantId.ToString();
            repo.UpdateKlant(created);
            repoUser.CreateGebruiker(email, naam, created.KlantId, RolType.Klant);
            return k;
        }
        public void BlockKlantAccount(int id)
        {
            Klant k = GetKlant(id);
            Gebruiker user = repoUser.FindGebruiker(id);
            user.Toegestaan = false;
            repoUser.UpdateGebruiker(user);
            repo.BlockKlant(id);
        }
        public void UnblockKlantAccount(int id)
        {
            Klant k = GetKlant(id);
            Gebruiker user = repoUser.FindGebruiker(id);
            user.Toegestaan = true;
            repoUser.UpdateGebruiker(user);
            repo.UnblockKlant(id);
        }

        public void RemoveKlant(int id)
        {
            Klant k = GetKlant(id);
            Gebruiker user = repoUser.FindGebruiker(id);
            if (k.IsKlantAccount == false)
            {
                repo.DeleteKlant(k);
                repoUser.DeleteGebruiker(user);
                List<Klant> klantenAcc = new List<Klant>();
                klantenAcc = GetKlanten().ToList();
                foreach (Klant acc in klantenAcc)
                {
                    if (acc.HoofdKlant == k)
                    {
                        Gebruiker userAcc = repoUser.FindGebruiker(acc.KlantId);
                        repo.DeleteKlant(acc);
                        repoUser.DeleteGebruiker(userAcc);
                    }
                }
            }
            else
            {

                repo.DeleteKlant(k);
                repoUser.DeleteGebruiker(user);
            }
        }
        public void RemoveKlantAccount(int id)
        {
            Klant k = GetKlant(id);
            Gebruiker user = repoUser.FindGebruiker(id);
            repo.DeleteKlant(k);
            repoUser.DeleteGebruiker(user);
        }


        // Nodig Voor Backup indien een fout voorkomt in nieuwere methode
        //public IEnumerable<Klant> GetHoofdKlanten()
        //{
        //    List<Klant> klantenAcc = new List<Klant>();
        //    List<Klant> klantenAccs = new List<Klant>();
        //    klantenAcc = GetKlanten().ToList();
        //    foreach (Klant acc in klantenAcc)
        //    {
        //        if (acc.IsKlantAccount == false)
        //        {
        //            klantenAccs.Add(acc);
        //        }
        //    }
        //    return klantenAccs;
        //}
        
    }
}
