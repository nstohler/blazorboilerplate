using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorBoilerplate.Server.Entities;
using Core.LibLog.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BlazorBoilerplate.Server.Data
{
    internal class CoreDataSeeder : ICoreDataSeeder
    {
        private static readonly ILog Logger = LogProvider.For<CoreDataSeeder>();

        private readonly IConfiguration                  _configuration;
        private readonly IWebHostEnvironment             _webHostEnvironment;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ApplicationDbContext            _context;

        public CoreDataSeeder(IConfiguration configuration, IWebHostEnvironment webHostEnvironment,
            RoleManager<IdentityRole<Guid>> roleManager, ApplicationDbContext context)
        {
            _configuration      = configuration;
            _webHostEnvironment = webHostEnvironment;
            _roleManager        = roleManager;
            _context            = context;
        }

        private string SeedDataJsonPath => $@"{_webHostEnvironment.WebRootPath}\DbSeedData";

        public async Task Seed()
        {
            var errMsg    = $"Error while resetting/seeding the database (CoreDataSeeder)";
            var stopwatch = Stopwatch.StartNew();

            try
            {
                Logger.Info("CoreDataSeeder::Seed start");

                //ApplyDeleteDatabase();
                //ApplyCreateDatabase();
                //ApplyMigrateDatabase();

                // Database should exist and be ready at this point: seed core data
                if (_configuration["Startup:RunDbSeeders"] == bool.TrueString)
                {
                    if (_configuration["Startup:SeedInitialData"] == bool.TrueString)
                    {
                        await SeedInitialData();
                    }

                    if (_configuration["Startup:SeedUserRoles"] == bool.TrueString)
                    {
                        await SeedUserRoles();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException("{errMsg}: {exMsg}", ex, errMsg, ex.Message);
                throw;
            }
            finally
            {
                Logger.InfoFormat("CoreDataSeeder::Seed end duration: {elapsed}", stopwatch.Elapsed);
            }
        }

        private async Task SeedInitialData()
        {
            Logger.Info("CoreDataSeeder::SeedInitialData");
            await Task.Delay(0);

            // TODO:
            // - load json data
            // - map using autoMapper
            // - add to db
            SeedCountries();

            _context.SaveChanges();
        }

        private void SeedCountries()
        {
            if (_context.Countries.Any())
            {
                return;
            }

            var filename = $@"{SeedDataJsonPath}\countries.json";
            var typedItems = JsonConvert.DeserializeObject<List<Country>>(File.ReadAllText(filename));

            _context.Countries.AddRange(typedItems);
        }

        private async Task SeedUserRoles()
        {
            Logger.Info("CoreDataSeeder::SeedUserRoles");

            // https://gooroo.io/GoorooTHINK/Article/17333/Custom-user-roles-and-rolebased-authorization-in-ASPNET-core/32835
            string[]       roleNames = {"SuperAdmin", "Admin", "User"};
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                //creating the roles and seeding them to the database
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                }
            }
        }
    }
}