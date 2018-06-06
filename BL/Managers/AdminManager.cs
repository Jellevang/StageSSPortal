using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Repositories;
using Domain;

namespace BL
{
    public class AdminManager : IAdminManager
    {
        private readonly AdminRepository repo;
        public AdminManager()
        {
            repo = new AdminRepository();
        }
        //Deze methode haalt de admin op.
        public Admin GetAdmin()
        {
            return repo.ReadAdmin();
        }
        //Deze methode updatet het OVMPasswoord.
        public void UpdatePasswd(string passwd, Admin admin)
        {
            repo.ChangePasswd(passwd, admin);
        }
        //Deze methode haalt het OVMPasswoord op.
        public string GetPasswd(Admin admin)
        {
            return repo.ReadPasswd(admin);
        }
    }
}