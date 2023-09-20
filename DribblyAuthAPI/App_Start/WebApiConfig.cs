using Dribbly.Service.Services;
using DribblyAuthAPI.Filters;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;

namespace DribblyAuthAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Register services for Dependency Injection
            UnityConfig.RegisterComponents(config);

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Global Filters
            config.Filters.Add(new DribblyExceptionsFilter((ILogsService)config.DependencyResolver.GetService(typeof(ILogsService))));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var corsAttr = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(corsAttr);
        }
    }
}
