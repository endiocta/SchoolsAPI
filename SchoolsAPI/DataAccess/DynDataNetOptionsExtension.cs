using Microsoft.EntityFrameworkCore.Infrastructure;
using SchoolsAPI.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace SchoolsAPI.DataAccess
{
    public class DynDataNetOptionsExtension : IDbContextOptionsExtension
    {
        private class DynDataNetExtensionInfo : DbContextOptionsExtensionInfo
        {
            private int? _serviceProviderHash;

            private new DynDataNetOptionsExtension Extension => (DynDataNetOptionsExtension)base.Extension;

            public override bool IsDatabaseProvider => false;

            public override string LogFragment => string.Empty;

            public DynDataNetExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            public override int GetServiceProviderHashCode()
            {
                if (!_serviceProviderHash.HasValue)
                {
                    HashCode hashCode = default(HashCode);
                    hashCode.Add(Extension.DBSetting);
                    _serviceProviderHash = hashCode.ToHashCode();
                }

                return _serviceProviderHash.Value;
            }

            public override void PopulateDebugInfo([NotNull] IDictionary<string, string> debugInfo)
            {
                debugInfo["a2n.DataAccess:DBSetting"] = HashCode.Combine(Extension.DBSetting).ToString(CultureInfo.InvariantCulture);
            }

            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            {
                return true;
            }
        }
        private DbContextOptionsExtensionInfo _info;
        public DbContextOptionsExtensionInfo Info => _info ?? (_info = new DynDataNetExtensionInfo(this));

        public DatabaseServer DBSetting { get; set; }

        //public DynDbContextEventHandler Handler { get; set; }

        public DynDataNetOptionsExtension()
        {
        }

        public DynDataNetOptionsExtension(DynDataNetOptionsExtension extension)
        {
            DBSetting = extension.DBSetting;
        }

        public void ApplyServices(IServiceCollection services)
        {
        }

        public void Validate(IDbContextOptions options)
        {
        }
    }
}
