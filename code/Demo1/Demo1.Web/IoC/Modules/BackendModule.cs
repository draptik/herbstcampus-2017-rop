using Autofac;
using Demo1.Backend.ApplicationServices;

namespace Demo1.Web.IoC.Modules
{
    public class BackendModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterAssemblyTypes(typeof(CustomerRegistration).Assembly)
                .AsImplementedInterfaces();
        }
    }
}