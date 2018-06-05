using Domain;
using System.Collections.Generic;

namespace DAL.Interfaces
{
    //Deze methodes worden uitgewerkt in de KlantRepository. De interface dient ervoor om te verzekeren dat alle methodes hier gedeclareerd zeker worden uitgewerkt. 
    public interface IKlantRepository
    {
        Klant CreateKlant(Klant Klant);
        Klant GetKlant(int id);
        Klant GetKlant(string email);
        Klant GetKlantByName(string naam);
        IEnumerable<Klant> ReadKlanten();
        Klant ReadHoofdKlant(int klantId);
        void UpdateKlant(Klant Klant);
        void BlockKlant(int id);
        void UnblockKlant(int id);
        void DeleteKlant(Klant Klant);
        IEnumerable<Klant> ReadHoofdKlanten();
    }
}
