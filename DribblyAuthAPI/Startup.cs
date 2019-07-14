//reference: http://bitoftech.net/2014/06/01/token-based-authentication-asp-net-web-api-2-owin-asp-net-identity/
using Microsoft.Owin;
using Owin;
using System.Web.Http;

[assembly: OwinStartup(typeof(DribblyAuthAPI.API.Startup))]
namespace DribblyAuthAPI.API
{
    public class Startup
    {
        /// <summary>
        /// The “Configuration” method accepts parameter of type “IAppBuilder” this parameter will be
        /// supplied by the host at run-time. This “app” parameter is an interface which will be used
        /// to compose the application for our Owin server
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            //The “HttpConfiguration” object is used to configure API routes, so we’ll pass this object
            //to method “Register” in “WebApiConfig” class.
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            //we’ll pass the “config” object to the extension method “UseWebApi” which will be responsible
            //to wire up ASP.NET Web API to our Owin server pipeline
            app.UseWebApi(config);
        }

    }
}