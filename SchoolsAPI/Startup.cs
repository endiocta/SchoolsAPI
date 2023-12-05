using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ANTAM_FI_AppSysV2.API", Version = "v1" });
//                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//                {
//                    Description =
//"JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
//                    Name = "Authorization",
//                    In = ParameterLocation.Header,
//                    Type = SecuritySchemeType.ApiKey,
//                    Scheme = "Bearer"
//                });
//                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
//                {
//                    {
//                        new OpenApiSecurityScheme
//                        {
//                            Reference = new OpenApiReference
//                            {
//                                Type = ReferenceType.SecurityScheme,
//                                Id = "Bearer"
//                            },
//                            Scheme = "oauth2",
//                            Name = "Bearer",
//                            In = ParameterLocation.Header,

//                        },
//                        new List<string>()
//                    }
//                });

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SchoolsAPI");
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            }); ;
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
