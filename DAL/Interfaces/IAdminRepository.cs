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
        Admin CreateAdmin(Admin admin);
        Admin GetAdmin(int id);
        IEnumerable<Admin> ReadAdmins();
        void UpdateAdmin(Admin admin);
        void DeleteAdmin(int id);
    }
}
