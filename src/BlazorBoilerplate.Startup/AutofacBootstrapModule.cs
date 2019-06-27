using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using BlazorBoilerplate.Startup.Helpers;
using Module = Autofac.Module;

namespace BlazorBoilerplate.Startup
{
    public class AutofacBootstrapModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // get assemblies (scan, hardcode...)
            var assemblies = GetAssemblies().ToList();

            foreach (var assembly in assemblies)
            {
                builder.RegisterAssemblyModules(assembly);
            }

            // autoMapper registration via assemblies
        }

        private IEnumerable<Assembly> GetAssemblies()
        {
            // var assemblies = new List<Assembly>();

            //var x = Assembly.LoadFile("BlazorBoilerplate.Bonus.dll");

            //var x = typeof(IBonusService).Assembly;
            //assemblies.Add(x);

            var excludeEndings = new string[]
            {
                // ".Server",
                ".Client",
                // ".Startup"
            };

            var assemblies = AssemblyScanner.GetAssemblies()
                    .Where(a => a.GetName().Name.StartsWith("BlazorBoilerplate."))
                    .Where(a =>
                    {
                        var name = a.GetName().Name;
                        return excludeEndings.All(e => !name.EndsWith(e));
                    })
                    .Where(a => a.GetName().FullName != typeof(AutofacBootstrapModule).Assembly.GetName().FullName) // prevent recursive loading of this module

                    //.Where(a => !a.GetName().Name.EndsWith(".Server"))      
                    //.Where(a => !a.GetName().Name.EndsWith(".Client"))
                    //.Where(a => !a.GetName().Name.EndsWith(".Startup"))
                    //.Where(a =>
                    //{
                    //    return a.GetName().Name.Contains(".Bonus") ||
                    //           a.GetName().Name.Contains(".Shared")
                    //        ;
                    //})
                ;

            return assemblies;
        }
    }
}
