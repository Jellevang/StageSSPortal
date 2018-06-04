using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IAdminManager
    {
        Admin GetAdmin();
        void UpdatePasswd(string passwd, Admin admin);
        string GetPasswd(Admin admin);
    }
}