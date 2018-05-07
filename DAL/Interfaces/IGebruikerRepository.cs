using System.Collections.Generic;
using Domain.Gebruikers;

namespace DAL.Interfaces
{
    public interface IGebruikerRepository
    {
        List<Gebruiker> ReadGebruikers();
        Gebruiker CreateGebruiker(string email, string naam, int gebruikerid, RolType rol);
        //IEnumerable<Gebruiker> ReadGebruikersAdmin();
        void UpdateGebruiker(Gebruiker user);
        Gebruiker FindGebruiker(int gebruikerId);
        void DeleteGebruiker(Gebruiker user);
    }
}
