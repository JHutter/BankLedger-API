using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ServiceStack.Configuration;
using BankLedgerAPI.Services;
using IUserService = BankLedgerAPI.Services.IUserService;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;

namespace BankLedgerAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // TODO figure out what non-deprecated method is for UserInMemoryDatabase
            services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase(databaseName: "Users"));

            // configure jwt authentication
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            services.AddScoped<IUserService, UserService>(); // register user service
            //var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("BLA_JWT_SECRET"));
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                        var userId = int.Parse(context.Principal.Identity.Name);
                        var user = userService.GetById(userId);
                        if (user == null)
                        {
                            context.Fail("Unauthorized");
                        }
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            //                .AddNewtonsoftJson();

            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });

            // source https://medium.com/cloudnimble/switching-to-system-text-json-in-net-core-3-0-not-so-fast-4c577a21298b
            services.AddMvc().AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
                    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNameCaseInsensitive = true);
            //MvcOptions.EnableEndPoiuntRouting = false;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            string appUrl = Environment.GetEnvironmentVariable("BLA_BASE_URL");

            if (String.IsNullOrWhiteSpace(appUrl))
            {
                appUrl = "https://localhost:5001/";
            }
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}