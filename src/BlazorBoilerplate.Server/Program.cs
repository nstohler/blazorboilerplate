using System;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using BlazorBoilerplate.Server.Data;
using Core.LibLog.Logging;
using EnsureThat;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Microsoft.Extensions.Logging;

namespace BlazorBoilerplate.Server
{
    public class Program
    {
        private static ILog _logger;

        public static async Task<int> Main(string[] args)
        {
            var appsettingsEnvName =
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development
                    ? "development"
                    : "production";

            // serilog: read config from appsettings.{env}.json
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.{appsettingsEnvName}.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            // liblog startup test
            _logger = LogProvider.For<Program>();
            _logger.Info("Startup (via LibLog interface)");

            try
            {
                Log.Information("Starting web server host");

                var webHostBuilder = CreateWebHostBuilder(args);
                var webHost        = webHostBuilder.Build();

                // Seed etc here:
                await Seed(webHost);

                webHost.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
        }

        private static async Task Seed(IWebHost webHost)
        {
            // https://wildermuth.com/2018/01/10/Re-thinking-Running-Migrations-and-Seeding-in-ASP-NET-Core-2-0

            using (var scope = webHost.Services.CreateScope())
            {
                var hostingEnvironment = scope.ServiceProvider.GetService<IWebHostEnvironment>();
                EnsureArg.IsNotNull(hostingEnvironment);

                var config = scope.ServiceProvider.GetService<IConfiguration>();
                EnsureArg.IsNotNull(config);

                if (hostingEnvironment.IsDevelopment() || config["Startup:RunDbSeeders"] == bool.TrueString)
                {
                    // Dev/Debug: seed user roles and other data here
                    var coreDataSeeder = scope.ServiceProvider.GetService<ICoreDataSeeder>();
                    await coreDataSeeder.Seed();
                }
            }
        }

        // https://autofaccn.readthedocs.io/en/latest/integration/aspnetcore.html
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            // use default configuration, here's whats in that one:
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.2#default-configuration

            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();

            return host;
        }
    }
}