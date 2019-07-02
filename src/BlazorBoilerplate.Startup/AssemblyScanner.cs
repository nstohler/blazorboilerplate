using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace BlazorBoilerplate.Startup
{
    public class AssemblyScanner
    {
        public static List<Assembly> GetFilteredAssemblies()
        {
            var assemblyFilters = new List<string>(StartupConstants.AssemblyFilters);

            var allAssemblies = GetAssemblies().ToList();

            // extend assembly filters:
            // - check assembly name endings (allowed/excluded)
            var extendAssemblyFilters = allAssemblies
                .Where(a => StartupConstants.AssemblyNameEndings.Any(ane =>
                {
                    return a.GetName().Name.EndsWith(ane, StringComparison.OrdinalIgnoreCase);
                }))
                .Where(a => StartupConstants.ExcludeAssemblyNameEndings.All(excludeName =>
                {
                    return !a.GetName().Name.EndsWith(excludeName, StringComparison.OrdinalIgnoreCase);
                }))
                .Select(a => a.GetName().Name.Split('.').First())
                .Distinct()
                .ToList();

            assemblyFilters.AddRange(extendAssemblyFilters.Except(assemblyFilters));

            var assemblies = allAssemblies
                .Where(
                    a => assemblyFilters.Any(f => a.GetName().Name.StartsWith(f, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            // additional: 
            // - load implementation assemblies "xxx" where "xxx.Interfaces" assembly is already loaded
            // - add to assemblies list
            var interfaces = ".Interfaces";
            var interfaceAssemblies = allAssemblies
                .Where(a => StartupConstants.AssemblyNameEndings.Any(ane =>
                    a.GetName().Name.EndsWith(interfaces, StringComparison.OrdinalIgnoreCase)))
                .Distinct()
                .ToList();

            foreach (var interfaceAssembly in interfaceAssemblies)
            {
                // remove ".Interfaces" from dll path
                var location = ReplaceLastOccurrence(interfaceAssembly.Location, interfaces, "");

                // only add to list if that location has not alreay been loaded (= included and referenced by multiple projects), 
                // so the autfacRegistrationModule will be detected
                if (!assemblies.Exists(a => a.Location == location))
                {
                    // explicitly load the assembly                
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(location);

                    assemblies.Add(assembly);
                }
            }

            //// AutoMapper.Profile
            //var autoMapperAssemblies = assemblies
            //    .Where(a => HasDerivedTypes(a, typeof(AutoMapper.Profile)))
            //    .ToList();

            //// Autofac.Module
            //var autoFacAssemblies = assemblies
            //    .Where(a => HasDerivedTypes(a, typeof(Autofac.Module)))
            //    .ToList();             

            //// filter CONTAINED in assembly name  (fails for "Server", adds some aspnet.core assemblies...)
            //var assemblies = GetAssemblies()
            //    .Where(a => assemblyFilters.Any(f => a.FullName.Contains(f, StringComparison.OrdinalIgnoreCase)));

            return assemblies.ToList();
        }


        // http://stackoverflow.com/a/10253634/54159
        private static IEnumerable<Assembly> GetAssemblies()
        {
            var list  = new List<string>();
            var stack = new Stack<Assembly>();

            stack.Push(Assembly.GetEntryAssembly());

            do
            {
                var asm = stack.Pop();

                yield return asm;

                foreach (var reference in asm.GetReferencedAssemblies())
                    if (!list.Contains(reference.FullName))
                    {
                        stack.Push(Assembly.Load(reference));
                        list.Add(reference.FullName);
                    }
            } while (stack.Count > 0);
        }

        private static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find, StringComparison.OrdinalIgnoreCase);

            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }
    }
}