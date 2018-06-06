using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    //Deze methodes worden uitgewerkt in de Adminmanager. De interface dient ervoor om te verzekeren dat alle methodes hier gedeclareerd zeker worden uitgewerkt. 
    public interface IAdminManager
    {
        Admin GetAdmin();
        void UpdatePasswd(string passwd, Admin admin);
        string GetPasswd(Admin admin);
    }
}