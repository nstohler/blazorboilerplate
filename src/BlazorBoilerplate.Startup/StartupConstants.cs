using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorBoilerplate.Startup
{
    public static class StartupConstants
    {
        // TODO: populate this from appsettings?
        // => try that next!

        public static readonly List<string> AssemblyFilters = new List<string>()
        {
            "Core"
        };

        public static readonly List<string> AssemblyNameEndings = new List<string>()
        {
            ".Config",
            ".Controllers",            
            ".Data",
            ".Data.Interfaces",
            ".Domain",
            ".Models",
            ".Services",
            ".ViewModels",
            ".Auth",
            ".Claims",
            ".Identity"
        };

        public static readonly List<string> ExcludeAssemblyNameEndings = new List<string>
        {
            ".Client",
        };

        // static constructor
        static StartupConstants()
        {
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
    }
}
