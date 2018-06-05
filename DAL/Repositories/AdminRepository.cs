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
        //In deze methode lezen we de Admin uit de databank.
        public Admin ReadAdmin()
        {
            Admin a = _ctx.Admins.FirstOrDefault();
            return a;
        }
        //Met deze methode kan je een Admin creëren.
        public Admin CreateAdmin(Admin admin)
        {
            _ctx.Admins.Add(admin);
            _ctx.SaveChanges();
            return admin;
        }
        //Hier halen we een Admin op aan de hand van een id.
        public Admin GetAdmin(int id)
        {
            Admin a = _ctx.Admins.Find(id);
            return a;
        }
        //Hier decrypten en halen we het Oracle Manager passwoord op.
        //We doen dit door een Admin mee te geven en zijn Oracle Manager passwoord te decrypten.
        public string ReadPasswd(Admin admin)
        {
            string passwd = admin.OvmPassword;
            string decryptedPasswd = Encrypt.DecryptString(passwd);
            return decryptedPasswd;
        }
        //Hier passen we het Oracle Manager Passwoord aan van de Admin.
        //Dit doen we door een nieuw passwoord mee te geven en het dan te encrypten.
        public void ChangePasswd(string passwd, Admin admin)
        {
            string encryptstring = Encrypt.EncryptString(passwd);
            admin.OvmPassword = encryptstring;
            _ctx.SaveChanges();
        }
        //Methode om de admin te updaten.
        public void UpdateAdmin(Admin admin)
        {
            _ctx.Entry(admin).State = EntityState.Modified;
            _ctx.SaveChanges();
        }
        //Methode om de Admin te deleten
        public void DeleteAdmin(int id)
        {
            Admin a = _ctx.Admins.Find(id);
            _ctx.Admins.Remove(a);
            _ctx.SaveChanges();
        }
    }
}
