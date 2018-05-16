using DAL.EF;
using DAL.Interfaces;
using Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly StageSSPortalDbContext _ctx;
        public AdminRepository()
        {
            _ctx = new StageSSPortalDbContext();
        }

        /*public AdminRepository(UnitOfWork uow): base(uow.Context)
        {
            _ctx = uow.Context;
        }*/
        public Admin ReadAdmin()
        {
            Admin a = _ctx.Admins.FirstOrDefault();
            return a;
        }

        public Admin CreateAdmin(Admin admin)
        {
            _ctx.Admins.Add(admin);
            _ctx.SaveChanges();
            return admin;
        }

        public Admin GetAdmin(int id)
        {
            Admin a = _ctx.Admins.Find(id);
            return a;
        }
        public string ReadPasswd(Admin admin)
        {
            string passwd = admin.OvmPassword;
            string decryptedPasswd = Encrypt.DecryptString(passwd);
            return decryptedPasswd;
        }
        public void ChangePasswd(string passwd, Admin admin)
        {
            //string plaintext = "monin";
            string encryptstring = Encrypt.EncryptString(passwd);
            admin.OvmPassword = encryptstring;
            _ctx.SaveChanges();
        }

        public IEnumerable<Admin> ReadAdmins()
        {
            return _ctx.Admins.AsEnumerable();
        }

        public void UpdateAdmin(Admin admin)
        {
            _ctx.Entry(admin).State = EntityState.Modified;
            _ctx.SaveChanges();
        }

        public void DeleteAdmin(int id)
        {
            Admin a = _ctx.Admins.Find(id);
            _ctx.Admins.Remove(a);
            _ctx.SaveChanges();
        }
    }
}
