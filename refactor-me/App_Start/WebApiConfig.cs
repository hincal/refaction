using refactor_me.Database;
using refactor_me.Repositories;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using System.Configuration;
using System.Web.Http;

namespace refactor_me
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //  SimpleInjector definitions
            var container = new Container();

            //  Register DbConnectionFactory to SimpleInjector
            var connectionString = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
            container.Register<IDbConnectionFactory>(() => new DbConnectionFactory(connectionString));

            //  Register other instances
            container.Register<IProductRepository, ProductRepository>();
            container.Register<IProductOptionRepository, ProductOptionRepository>();
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);
            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);

            // Web API configuration and services
            var formatters = GlobalConfiguration.Configuration.Formatters;
            formatters.Remove(formatters.XmlFormatter);
            formatters.JsonFormatter.Indent = true;

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
