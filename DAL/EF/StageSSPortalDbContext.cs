using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;
using Domain.Gebruikers;
using Domain;

namespace DAL.EF
{
    [DbConfigurationType(typeof(StageSSPortalDbConfiguration))]
    public partial class StageSSPortalDbContext : IdentityDbContext<Gebruiker>
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Klant> Klanten { get; set; }

        public StageSSPortalDbContext() : base("dbStageMonin")
        {
            //Database.SetInitializer<VerkiezingstestContext>(new VerkiezingstestInitializer());
            this.Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            // primary keys
            modelBuilder.Entity<Admin>().HasKey(a => a.AdminId);
            modelBuilder.Entity<Klant>().HasKey(k => k.KlantId);

            // required properties
            modelBuilder.Entity<Admin>().Property(a => a.Email).IsRequired();
            modelBuilder.Entity<Klant>().Property(k => k.Email).IsRequired();
            modelBuilder.Entity<Klant>().Property(k => k.Naam).IsRequired();

            // Identity
            modelBuilder.Entity<IdentityUserRole>().HasKey(i => i.UserId);
            modelBuilder.Entity<IdentityUserLogin>().HasKey(i => i.UserId);


        }
    }
}
