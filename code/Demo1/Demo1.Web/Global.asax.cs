using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Demo1.Web.IoC;
using Demo1.Backend.Infrastructure;

namespace Demo1.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var mapper = AutomapperConfiguration.Init();
            IoCBuilder.InitWeb(Assembly.GetExecutingAssembly(), mapper);
        }
    }
}