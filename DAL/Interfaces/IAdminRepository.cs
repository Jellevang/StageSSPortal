using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    //Deze methodes worden uitgewerkt in de AdminRepository. De interface dient ervoor om te verzekeren dat alle methodes hier gedeclareerd zeker worden uitgewerkt. 
    public interface IAdminRepository
    {
        string ReadPasswd(Admin admin);
        Admin CreateAdmin(Admin admin);
        Admin ReadAdmin();
        void ChangePasswd(string hash, Admin admin);
        void UpdateAdmin(Admin admin);
        void DeleteAdmin(int id);
    }
}
