using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EnsureThat;

namespace BlazorBoilerplate.Startup
{
    public static class Bootstrapper
    {
        private static IContainer _autofacContainer = null;

        public static IContainer Initialize(Action<ContainerBuilder> populateWithServices = null)
        {
            EnsureArg.IsTrue(_autofacContainer == null, nameof(_autofacContainer));

            var assemblies = AutofacBootstrapModule.GetAssemblies().ToList();

            _autofacContainer = Bootstrapper.CreateContainer(assemblies, builder =>
            {
                populateWithServices?.Invoke(builder); // populate with ASP.NET Core registered services

                AutoMapperConfig.RegisterForDI(builder, assemblies, () => _autofacContainer);
            });

            EnsureArg.IsNotNull(_autofacContainer, nameof(_autofacContainer));

            return _autofacContainer;
        }

        private static IContainer CreateContainer(List<Assembly> assemblies,
            Action<ContainerBuilder> containerBuilder)
        {
            return AutofacLoader.Initialize(assemblies, containerBuilder);
        }
    }
}