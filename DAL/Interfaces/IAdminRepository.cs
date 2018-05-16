using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IAdminRepository
    {
        string ReadPasswd(Admin admin);
        Admin CreateAdmin(Admin admin);
        Admin ReadAdmin();
        IEnumerable<Admin> ReadAdmins();
        void ChangePasswd(string hash, Admin admin);
        void UpdateAdmin(Admin admin);
        void DeleteAdmin(int id);
    }
}
