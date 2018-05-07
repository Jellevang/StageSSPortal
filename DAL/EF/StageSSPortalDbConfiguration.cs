using MySql.Data.Entity;

namespace DAL.EF
{
    //internal class StageSSPortalDbConfiguration :  DbConfiguration
    internal class StageSSPortalDbConfiguration : MySqlEFConfiguration
    {
        public StageSSPortalDbConfiguration()
        {
            this.SetDefaultConnectionFactory(new System.Data.Entity.Infrastructure.SqlConnectionFactory());
            this.SetProviderServices("System.Data.SqlClient", System.Data.Entity.SqlServer.SqlProviderServices.Instance);
            this.SetDatabaseInitializer<StageSSPortalDbContext>(new StageSSPortalDbInitializer());
        }
    }
}
