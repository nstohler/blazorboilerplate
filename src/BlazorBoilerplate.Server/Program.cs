using System;
using Autofac.Extensions.DependencyInjection;
using Core.LibLog.Logging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlazorBoilerplate.Server
{
    public class Program
    {
        private static ILog _logger;

        public static int Main(string[] args)
        {
            var appsettingsEnvName =
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development
                    ? "development"
                    : "production";

            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.{appsettingsEnvName}.json", true, true)
                //"appsettings" + (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development ? ".Development.json" : ".json"))
                // .AddJsonFile("appsettings.Production.json", true, true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            // liblog
            _logger = LogProvider.For<Program>();
            _logger.Info("Startup (via LibLog interface)");

            try
            {
                Log.Information("Starting web server host");
                BuildWebHost(args).Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
        }

        // https://autofaccn.readthedocs.io/en/latest/integration/aspnetcore.html
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(new ConfigurationBuilder()
                    .AddCommandLine(args)
                    .Build())
                // .ConfigureServices(services => services.AddAutofac())
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();

        //private static void ConfigureLogger(WebHostBuilderContext hostingContext, ILoggingBuilder logging)
        //{
        //    // https://itnext.io/loggly-in-asp-net-core-using-serilog-dc0e2c7d52eb
        //    logging.ClearProviders(); // disable asp.net core logging

        //    //// https://stackoverflow.com/a/52400244/54159
        //    //// disable hangfire logging to serilog/liblog             
        //    //GlobalConfiguration.Configuration.UseLogProvider(new NoLoggingProvider());
        //    // => above did not work,

        //    // but adding "Hangfire": "Warning" into appsettings.json | Serilog | MinimumLevel | Override works!
        //    // https://stackoverflow.com/a/46204891/54159

        //    // How to turn off the logging done by the ASP.NET core framework:
        //    // https://stackoverflow.com/a/46204891/54159

        //    var configuration = hostingContext.Configuration;
        //    logging.AddConfiguration(configuration.GetSection("Logging"));
        //    logging.AddConsole();
        //    // loggerFactory.AddDebug();
        //    logging.AddSerilog();

        //    // serilog stuff now here:
        //    //---
        //    // configure via appsettings.json:
        //    // => https://github.com/serilog/serilog-settings-configuration
        //    // => https://github.com/serilog/serilog-sinks-rollingfile

        //    // -------------------------------
        //    //  To prevent duplicate logfiles 
        //    //     (might have been an issue with serilog.sinks.rollingfile, switched to serilog.sinkgs.file now!)
        //    // -------------------------------
        //    // https://www.taithienbo.com/how-to-auto-start-and-keep-an-asp-net-core-web-application-and-keep-it-running-on-iis/
        //    // - remove "Serilog" settings branch from appsettings.json
        //    //   => configure only ONCE in whole settings tree (appsettings.Development.json / appsettings.Production.json)
        //    // - IIS configuration:
        //    //   - AppPool -> Advanced Settings -> Start Mode      = AlwaysRunning
        //    //   - Site    -> Advanced Settings -> Preload Enabled = False          
        //    // -------------------------------

        //    Log.Logger = new LoggerConfiguration()
        //        .ReadFrom.Configuration(configuration)
        //        .CreateLogger();

        //    //// explicitly create logger (for debugging)
        //    //Log.Logger = new LoggerConfiguration()
        //    //    .WriteTo.File(@"log\MeteoFailover.Web-.log", 
        //    //            rollingInterval: RollingInterval.Day, 
        //    //            fileSizeLimitBytes: 209715200,
        //    //            rollOnFileSizeLimit: true,
        //    //            retainedFileCountLimit: 100,
        //    //            restrictedToMinimumLevel: LogEventLevel.Debug)
        //    //        .WriteTo.Seq("http://localhost:5341", restrictedToMinimumLevel: LogEventLevel.Information)
        //    //        .CreateLogger();

        //    Log.Logger.Information("Serilog initialized");

        //    // liblog
        //    _logger = LogProvider.For<Program>();
        //    _logger.Info("Startup (via LibLog interface)");
        //}
    }
}