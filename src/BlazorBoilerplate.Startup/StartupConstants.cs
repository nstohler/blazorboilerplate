using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BlazorBoilerplate.Startup
{
    public static class StartupConstants
    {
        private const string BotstrapSettingsJsonFilename = "bootstrapSettings.json";

        public static readonly List<string> AssemblyFilters;
        public static readonly List<string> AssemblyNameEndings;
        public static readonly List<string> ExcludeAssemblyNameEndings;

        // static constructor
        static StartupConstants()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(BotstrapSettingsJsonFilename, false)
                .Build();

            AssemblyFilters = ReadConfigStrings(configuration, "AssemblyScanner:AssemblyFilters").AddLeadingDot();
            AssemblyNameEndings =
                ReadConfigStrings(configuration, "AssemblyScanner:AssemblyNameEndings").AddLeadingDot();
            ExcludeAssemblyNameEndings = ReadConfigStrings(configuration, "AssemblyScanner:ExcludeAssemblyNameEndings")
                .AddLeadingDot();

            // auto add .Startup and .Web (executing web assembly) base name to AssemblyFilters
            var thisAssemblyName = typeof(StartupConstants).Assembly.GetName().Name;
            var webAssemblyName  = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

            var assemblyNames = new List<string>()
            {
                thisAssemblyName,
                webAssemblyName
            };

            var projectNames = assemblyNames
                .Select(a => a.Split('.').First())
                .Distinct()
                .ToList();

            // merge in found items (but keep distinct)
            AssemblyFilters.AddRange(projectNames.Except(AssemblyFilters));
        }

        private static List<string> ReadConfigStrings(IConfiguration configuration, string key)
        {
            // https://stackoverflow.com/a/50727648/54159
            var strings = new List<string>();
            configuration.Bind(key, strings);
            return strings;
        }

        private static List<string> AddLeadingDot(this List<string> input)
        {
            return input.Select(x => $".{x}")
                .ToList();
        }
    }
}