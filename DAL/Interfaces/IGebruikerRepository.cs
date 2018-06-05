using System.Collections.Generic;
using Domain.Gebruikers;

namespace DAL.Interfaces
{
    //Deze methodes worden uitgewerkt in de GebruikerRepository. De interface dient ervoor om te verzekeren dat alle methodes hier gedeclareerd zeker worden uitgewerkt. 
    public interface IGebruikerRepository
    {
        List<Gebruiker> ReadGebruikers();
        Gebruiker CreateGebruiker(string email, string naam, int gebruikerid, RolType rol);
        void UpdateGebruiker(Gebruiker user);
        Gebruiker FindGebruiker(int gebruikerId);
        void DeleteGebruiker(Gebruiker user);
    }
}
