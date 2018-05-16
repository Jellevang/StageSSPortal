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
        public Admin GetAdmin()
        {
            return repo.ReadAdmin();
        }

        public void UpdatePasswd(string passwd, Admin admin)
        {
            repo.ChangePasswd(passwd, admin);
        }

        public string GetPasswd(Admin admin)
        {
            return repo.ReadPasswd(admin);
        }

    }
}