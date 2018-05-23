namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Admin",
                c => new
                    {
                        AdminId = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, unicode: false),
                        naam = c.String(unicode: false),
                        OvmPassword = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.AdminId);
            
            CreateTable(
                "dbo.Klant",
                c => new
                    {
                        KlantId = c.Int(nullable: false, identity: true),
                        Naam = c.String(nullable: false, unicode: false),
                        Email = c.String(nullable: false, unicode: false),
                        IsGeblokkeerd = c.Boolean(nullable: false),
                        IsKlantAccount = c.Boolean(nullable: false),
                        HoofdKlant_KlantId = c.Int(),
                    })
                .PrimaryKey(t => t.KlantId)
                .ForeignKey("dbo.Klant", t => t.HoofdKlant_KlantId)
                .Index(t => t.HoofdKlant_KlantId);
            
            CreateTable(
                "dbo.LogLijst",
                c => new
                    {
                        LogLijstId = c.Int(nullable: false, identity: true),
                        Naam = c.String(nullable: false, unicode: false),
                        GebruikerId = c.Int(nullable: false),
                        OvmId = c.String(nullable: false, unicode: false),
                        ActionDate = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.LogLijstId);
            
            CreateTable(
                "dbo.OracleVirtualMachine",
                c => new
                    {
                        OracleVirtualMachineId = c.Int(nullable: false, identity: true),
                        Naam = c.String(nullable: false, unicode: false),
                        OvmId = c.String(nullable: false, unicode: false),
                        KlantId = c.Int(nullable: false),
                        ServerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OracleVirtualMachineId);
            
            CreateTable(
                "dbo.OVMLijst",
                c => new
                    {
                        OVMLijstId = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        OVMId = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.OVMLijstId);
            
            CreateTable(
                "dbo.IdentityRole",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        Name = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IdentityUserRole",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        RoleId = c.String(unicode: false),
                        IdentityRole_Id = c.String(maxLength: 128, storeType: "nvarchar"),
                        Gebruiker_Id = c.String(maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.IdentityRole", t => t.IdentityRole_Id)
                .ForeignKey("dbo.Gebruiker", t => t.Gebruiker_Id)
                .Index(t => t.IdentityRole_Id)
                .Index(t => t.Gebruiker_Id);
            
            CreateTable(
                "dbo.Server",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ServerNaam = c.String(nullable: false, unicode: false),
                        ServersId = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Gebruiker",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        GebruikerId = c.Int(nullable: false),
                        Naam = c.String(unicode: false),
                        Rol = c.Int(nullable: false),
                        Toegestaan = c.Boolean(nullable: false),
                        MustChangePassword = c.Boolean(nullable: false),
                        LastPasswordChangedDate = c.DateTime(nullable: false, precision: 0),
                        Email = c.String(unicode: false),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(unicode: false),
                        SecurityStamp = c.String(unicode: false),
                        PhoneNumber = c.String(unicode: false),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(precision: 0),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IdentityUserClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(unicode: false),
                        ClaimType = c.String(unicode: false),
                        ClaimValue = c.String(unicode: false),
                        Gebruiker_Id = c.String(maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Gebruiker", t => t.Gebruiker_Id)
                .Index(t => t.Gebruiker_Id);
            
            CreateTable(
                "dbo.IdentityUserLogin",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        LoginProvider = c.String(unicode: false),
                        ProviderKey = c.String(unicode: false),
                        Gebruiker_Id = c.String(maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Gebruiker", t => t.Gebruiker_Id)
                .Index(t => t.Gebruiker_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IdentityUserRole", "Gebruiker_Id", "dbo.Gebruiker");
            DropForeignKey("dbo.IdentityUserLogin", "Gebruiker_Id", "dbo.Gebruiker");
            DropForeignKey("dbo.IdentityUserClaim", "Gebruiker_Id", "dbo.Gebruiker");
            DropForeignKey("dbo.IdentityUserRole", "IdentityRole_Id", "dbo.IdentityRole");
            DropForeignKey("dbo.Klant", "HoofdKlant_KlantId", "dbo.Klant");
            DropIndex("dbo.IdentityUserLogin", new[] { "Gebruiker_Id" });
            DropIndex("dbo.IdentityUserClaim", new[] { "Gebruiker_Id" });
            DropIndex("dbo.IdentityUserRole", new[] { "Gebruiker_Id" });
            DropIndex("dbo.IdentityUserRole", new[] { "IdentityRole_Id" });
            DropIndex("dbo.Klant", new[] { "HoofdKlant_KlantId" });
            DropTable("dbo.IdentityUserLogin");
            DropTable("dbo.IdentityUserClaim");
            DropTable("dbo.Gebruiker");
            DropTable("dbo.Server");
            DropTable("dbo.IdentityUserRole");
            DropTable("dbo.IdentityRole");
            DropTable("dbo.OVMLijst");
            DropTable("dbo.OracleVirtualMachine");
            DropTable("dbo.LogLijst");
            DropTable("dbo.Klant");
            DropTable("dbo.Admin");
        }
    }
}
