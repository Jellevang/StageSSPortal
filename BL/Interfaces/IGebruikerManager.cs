using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Gebruikers;

namespace BL
{
    //Deze methodes worden uitgewerkt in de GebruikerManager. De interface dient ervoor om te verzekeren dat alle methodes hier gedeclareerd zeker worden uitgewerkt. 
    public interface IGebruikerManager
    {
        List<Gebruiker> GetGebruikers();
        Gebruiker GetGebruiker(string username);
        Gebruiker GetGebruiker(int gebruikersid);
    }
}
