using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF
{
    internal class StageSSPortalDbConfiguration :  DbConfiguration
    {
        public StageSSPortalDbConfiguration()
        {
            this.SetDefaultConnectionFactory(new System.Data.Entity.Infrastructure.SqlConnectionFactory());
            this.SetProviderServices("System.Data.SqlClient", System.Data.Entity.SqlServer.SqlProviderServices.Instance);
            this.SetDatabaseInitializer<StageSSPortalDbContext>(new StageSSPortalDbInitializer());

        }
    }
}
