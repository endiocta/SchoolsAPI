using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SchoolsAPI.Configuration
{
    public class AppSettings
    {
        public DatabaseServer MainDB { get; set; }
    }

    public class DatabaseServer
    {
        public string ConnectionString { get; set; }
        public string DefaultSchema { get; set; } = "";

        [JsonConverter(typeof(StringEnumConverter))]
        public DatabaseProvider Provider { get; set; }
    }

    public enum DatabaseProvider
    {
        SqlServer
    }
}
