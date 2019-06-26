using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using BlazorBoilerplate.Server.AutofacTest;

namespace BlazorBoilerplate.Server
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<Tester>()
                .As<ITester>()
                .InstancePerLifetimeScope()
                //.SingleInstance()
                ;
        }
    }
}
