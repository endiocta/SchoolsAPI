using Microsoft.EntityFrameworkCore;
using SchoolsAPI.Configuration;
using SchoolsAPI.DataAccess;

namespace SchoolsAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = new AppSettings();
            Configuration.Bind("MainConfig", appSettings);
            services.AddSingleton<AppSettings>(appSettings);

            ApiDbContext.Open(appSettings.MainDB, db =>
            {
                Console.WriteLine("Checking Database...");
                if (db.Database.EnsureCreated())
                {
                    Console.WriteLine(@"Created new database: 
Provider        : {0}
Database Name   : {1}", appSettings.MainDB.Provider.ToString(), db.Database.GetDbConnection().Database);
                }
                else
                {
                    Console.WriteLine(@"Use existing database: 
Provider        : {0}
Database Name   : {1}", appSettings.MainDB.Provider.ToString(), db.Database.GetDbConnection().Database);
                }
            }, false);
            services.AddDbContext<ApiDbContext>(o =>
            {
                o.UseDynData(appSettings.MainDB);
            });

            services.AddControllers()
                .AddNewtonsoftJson(x =>
                {
                    x.UseMemberCasing();
                    x.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
