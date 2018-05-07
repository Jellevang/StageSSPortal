using Domain;
using System.Collections.Generic;

namespace DAL.Interfaces
{
    public interface IKlantRepository
    {
        Klant CreateKlant(Klant Klant);
        Klant GetKlant(int id);
        Klant GetKlant(string email);
        IEnumerable<Klant> ReadKlanten();
        void UpdateKlant(Klant Klant);
        void BlockKlant(int id);
        void UnblockKlant(int id);
        void DeleteKlant(Klant Klant);
    }
}
