//reference: http://bitoftech.net/2014/06/01/token-based-authentication-asp-net-web-api-2-owin-asp-net-identity/
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Web.Http;

[assembly: OwinStartup(typeof(DribblyAuthAPI.API.Startup))]
namespace DribblyAuthAPI.API
{
    //NOTE: No need to use Global.asax Class class and fire up the Application_Start event after we’ve
    //configured our “Startup” class so feel free to delete it.
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
            ConfigureOAuth(app);
            //The “HttpConfiguration” object is used to configure API routes, so we’ll pass this object
            //to method “Register” in “WebApiConfig” class.
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            //we’ll pass the “config” object to the extension method “UseWebApi” which will be responsible
            //to wire up ASP.NET Web API to our Owin server pipeline
            app.UseWebApi(config);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"), //http://localhost:[port]/token
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1), //set token validatity to 24 hours
                Provider = new SimpleAuthorizationServerProvider() //specifies implementation on how to validate the credentials for users asking for tokens
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        }

    }
}