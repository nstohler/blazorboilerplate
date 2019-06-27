using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using EnsureThat;

namespace BlazorBoilerplate.Startup
{
    public static class AutoMapperConfig
    {
        // inspired by https://www.jan-v.nl/post/using-dependency-injection-in-your-automapper-profile


        public static void RegisterForDI(ContainerBuilder builder, List<Assembly> assemblies,
            Func<IContainer> autofacContainerAccessFn)
        {
            // var f = autofacContainerAccessFn();
            builder.Register(ctx => AutoMapperConfig.GetConfig(assemblies, autofacContainerAccessFn));

            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper())
                .As<IMapper>()
                .InstancePerLifetimeScope()
                //.SingleInstance()
                ;
        }

        private static MapperConfiguration GetConfig(List<Assembly> assemblies, Func<IContainer> autofacContainerAccessFn)
        {
            // https://www.jan-v.nl/post/using-dependency-injection-in-your-automapper-profile

            var config = new MapperConfiguration(cfg =>
            {
                var autofacContainer = autofacContainerAccessFn();
                EnsureArg.IsNotNull(autofacContainer, nameof(autofacContainer));

                cfg.ConstructServicesUsing(autofacContainer.Resolve);

                cfg.AddMaps(assemblies); // register all AutoMapper.Profile derived types
            });

            config.AssertConfigurationIsValid();

            return config;
        }
    }
}