using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BankLedgerAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // base url set in env vars for portability, also test suite references same env var
            string baseUrl = System.Environment.GetEnvironmentVariable("BLA_BASE_URL");
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                baseUrl = "https://localhost:5001";
            }

            

            // source https://exceptionnotfound.net/ef-core-inmemory-asp-net-core-store-database/
            //1. Get the IWebHost which will host this application.
            var host = CreateWebHostBuilder(args)
                .UseKestrel()
                .UseUrls(baseUrl)
                .Build();

            //2. Find the service layer within our scope.
            using (var scope = host.Services.CreateScope())
            {
                //3. Get the instance of BoardGamesDBContext in our services layer
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<DataContext>();

                //4. Call the DataGenerator to create sample data
                DataGenerator.Initialize(services);
            }

            //Continue to run the application
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseDefaultServiceProvider(options =>
                    options.ValidateScopes = false); //https://stackoverflow.com/questions/48590579/cannot-resolve-scoped-service-from-root-provider-net-core-2
    }
}