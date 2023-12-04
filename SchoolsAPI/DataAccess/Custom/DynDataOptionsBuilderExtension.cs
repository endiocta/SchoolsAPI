using Microsoft.EntityFrameworkCore.Infrastructure;
using SchoolsAPI.Configuration;
using SchoolsAPI.DataAccess;

namespace Microsoft.EntityFrameworkCore
{
    public static class DynDataOptionsBuilderExtension
    {
        public static DbContextOptionsBuilder UseDynData(this DbContextOptionsBuilder optionsBuilder, DatabaseServer dbSetting)
        {
            DynDataNetOptionsExtension dynDataNetOptionsExtension = optionsBuilder.Options.FindExtension<DynDataNetOptionsExtension>() ?? new DynDataNetOptionsExtension();
            dynDataNetOptionsExtension.DBSetting = dbSetting;
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(dynDataNetOptionsExtension);
            optionsBuilder = optionsBuilder.UseSqlServer(dbSetting.ConnectionString);
            
            return optionsBuilder;
        }
    }
}
