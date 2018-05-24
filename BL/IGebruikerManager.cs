using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Gebruikers;

namespace BL
{
    public interface IGebruikerManager
    {
        List<Gebruiker> GetGebruikers();
        Gebruiker GetGebruiker(string username);
        Gebruiker GetGebruiker(int gebruikersid);
    }
}
