using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace BlazorBoilerplate.Startup
{
    public static class AutofacLoader
    {
        public static IContainer Initialize(List<Assembly> assemblies, Action<ContainerBuilder> containerBuilderFn)
        {
            ContainerBuilder builder = new ContainerBuilder();

            // register all Autofac.Module derived types
            foreach (var assembly in assemblies)
            {
                builder.RegisterAssemblyModules(assembly);
            }

            // invoke containerBuilderFn if provided
            containerBuilderFn?.Invoke(builder);

            var container = builder.Build();

            return container;
        }
    }
}
