using Domain;
using System.Collections.Generic;

namespace BL
{
    //Deze methodes worden uitgewerkt in de KlantManager. De interface dient ervoor om te verzekeren dat alle methodes hier gedeclareerd zeker worden uitgewerkt. 
    public interface  IKlantManager
    {
        Klant AddKlant(string naam, string email, string afkorting);
        void ChangeKlant(Klant Klant);
        void BlockKlant(int id);
        void UnblockKlant(int id);
        Klant GetKlant(int id);
        Klant GetKlant(string email);
        Klant GetKlantByName(string naam);
        IEnumerable<Klant> GetKlanten();
        IEnumerable<Klant> GetHoofdKlanten();
        Klant GetHoofdKlant(int klantId);
        IEnumerable<Klant> GetKlantenAccounts(Klant k);
        Klant AddKlantAccount(string naam, string email, Klant h);
        void BlockKlantAccount(int id);
        void UnblockKlantAccount(int id);
        void RemoveKlant(int id);
        void RemoveKlantAccount(int id);       
    }
}