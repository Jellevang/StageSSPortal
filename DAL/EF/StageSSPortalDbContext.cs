using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;
using Domain.Gebruikers;
using Domain;
using MySql.Data.Entity;

namespace DAL.EF
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public partial class StageSSPortalDbContext : IdentityDbContext<Gebruiker>
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Klant> Klanten { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<OracleVirtualMachine> OracleVirtualMachines { get; set; }
        public DbSet<OVMLijst> OVMLijsten { get; set; }
        public DbSet<LogLijst> LogLijsten { get; set; }
        public DbSet<ScheduledDownTime>  ScheduleTDLijst { get; set; }
         
        public StageSSPortalDbContext() : base("stage")
        {
           //Database.SetInitializer<StageSSPortalDbContext>(new StageSSPortalDbInitializer());
            this.Configuration.LazyLoadingEnabled= false;
        }

       protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            // primary keys
            modelBuilder.Entity<Admin>().HasKey(a => a.AdminId);
            modelBuilder.Entity<Klant>().HasKey(k => k.KlantId);
            modelBuilder.Entity<Server>().HasKey(s => s.Id);
            modelBuilder.Entity<OracleVirtualMachine>().HasKey(o => o.OracleVirtualMachineId);
            modelBuilder.Entity<OVMLijst>().HasKey(ovm => ovm.OVMLijstId);
            modelBuilder.Entity<LogLijst>().HasKey(l => l.LogLijstId);
            modelBuilder.Entity<ScheduledDownTime>().HasKey(s => s.id);
            // required properties
            modelBuilder.Entity<Admin>().Property(a => a.Email).IsRequired();
            modelBuilder.Entity<Klant>().Property(k => k.Email).IsRequired();
            modelBuilder.Entity<Klant>().Property(k => k.Naam).IsRequired();
            modelBuilder.Entity<Server>().Property(s => s.ServerNaam).IsRequired();
            modelBuilder.Entity<Server>().Property(s => s.ServersId).IsRequired();
            modelBuilder.Entity<ScheduledDownTime>().Property(s => s.Eind).IsRequired();
            modelBuilder.Entity<ScheduledDownTime>().Property(s => s.Start).IsRequired();
            modelBuilder.Entity<ScheduledDownTime>().Property(s => s.OvmId).IsRequired();

            //modelBuilder.Entity<KlantAccount>().Property(ka => ka.KlantId).IsRequired();
            modelBuilder.Entity<OracleVirtualMachine>().Property(o => o.Naam).IsRequired();
            modelBuilder.Entity<OracleVirtualMachine>().Property(o => o.KlantId).IsRequired();
            modelBuilder.Entity<OracleVirtualMachine>().Property(o => o.OvmId).IsRequired();
            modelBuilder.Entity<OVMLijst>().Property(ovm => ovm.AccountId).IsRequired();
            modelBuilder.Entity<OVMLijst>().Property(ovm => ovm.OVMId).IsRequired();
            modelBuilder.Entity<LogLijst>().Property(l => l.Naam).IsRequired();
            modelBuilder.Entity<LogLijst>().Property(l => l.GebruikerId).IsRequired();
            modelBuilder.Entity<LogLijst>().Property(l => l.OvmId).IsRequired();
            modelBuilder.Entity<LogLijst>().Property(l => l.ActionDate).IsRequired();
            //modelBuilder.Entity<Klant>().Property(k => k.Afkorting).IsRequired();
            // Identity
            modelBuilder.Entity<IdentityUserRole>().HasKey(i => i.UserId);
            modelBuilder.Entity<IdentityUserLogin>().HasKey(i => i.UserId);
          //  modelBuilder.Entity<Klant>().HasIndex(k => k.Afkorting).IsUnique();


        }  
    }
}
