using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using Demo1.Web.IoC.Modules;

namespace Demo1.Web.IoC
{
    public class IoCBuilder
    {
        private static IContainer _container;

        public static void InitWeb(Assembly webAssembly, IMapper mapper)
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(webAssembly).InstancePerDependency();

            builder.Register(x => mapper);
            builder.RegisterModule<BackendModule>();

            _container = builder.Build();

            AutofacDependencyResolver autofacDependencyResolver = new AutofacDependencyResolver(_container);
            DependencyResolver.SetResolver(autofacDependencyResolver);
        }
    }
}