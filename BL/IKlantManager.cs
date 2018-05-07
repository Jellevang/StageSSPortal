using Domain;
using System.Collections.Generic;

namespace BL
{
    public interface  IKlantManager
    {
        Klant AddKlant(string naam, string email);
        void ChangeKlantprofiel(Klant p);
        void ChangeKlant(Klant Klant);
        void BlockKlant(int id);
        void UnblockKlant(int id);
        Klant GetKlant(int id);
        Klant GetKlant(string email);
        IEnumerable<Klant> GetKlanten();
        IEnumerable<Klant> GetKlantenAccounts(Klant k);
        Klant AddKlantAccount(string naam, string email, Klant h);
        void BlockKlantAccount(int id);
        void UnblockKlantAccount(int id);
        void RemoveKlant(int id);
        void RemoveKlantAccount(int id);
    }
}
