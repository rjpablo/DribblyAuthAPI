﻿//reference: http://bitoftech.net/2014/06/01/token-based-authentication-asp-net-web-api-2-owin-asp-net-identity/
using Dribbly.Chat.Resolvers;
using Dribbly.Model;
using Dribbly.Service;
using Dribbly.Service.Hubs;
using Dribbly.Service.Providers;
using Dribbly.Service.Services;
using Dribbly.SMS.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Owin;
using System;
using System.Data.Entity;
using System.Web.Http;

[assembly: OwinStartup(typeof(DribblyAuthAPI.API.Startup))]
namespace DribblyAuthAPI.API
{
    //NOTE: No need to use Global.asax Class class and fire up the Application_Start event after we’ve
    //configured our “Startup” class so feel free to delete it.
    public class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public static FacebookAuthenticationOptions facebookAuthOptions { get; private set; }
        public static GoogleOAuth2AuthenticationOptions GoogleAuthOptions { get; private set; }

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
            //Allow CORS
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            //we’ll pass the “config” object to the extension method “UseWebApi” which will be responsible
            //to wire up ASP.NET Web API to our Owin server pipeline
            app.UseWebApi(config);
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AuthContext, Migrations.Configuration>());

            #region SignalR
            app.MapSignalR();
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new SignalRContractResolver();
            var serializer = JsonSerializer.Create(settings);
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);
            #endregion

            SMSService.Init();
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(AuthContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            //use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

            //Configure Facebook External Login
            facebookAuthOptions = new FacebookAuthenticationOptions()
            {
                // App Name: FreeHoops Test 2
                AppId = "271234698173280",
                AppSecret = "15b745dced3617c9832032c36f7bf97f",
                Provider = new FacebookAuthProvider()
            };
            app.UseFacebookAuthentication(facebookAuthOptions);
            GoogleAuthOptions = new GoogleOAuth2AuthenticationOptions
            {
                ClientId = "631124985942-8p2ut6fueiu72olbnl90gm29il7bcv0c.apps.googleusercontent.com",
                ClientSecret = "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",
                Provider = new GoogleAuthProvider()
            };
            app.UseGoogleAuthentication(GoogleAuthOptions);

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"), //http://localhost:[port]/token
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(3), //set token validatity to 30 minutes
                Provider = new SimpleAuthorizationServerProvider(), //specifies implementation on how to validate the credentials for users asking for tokens
                RefreshTokenProvider = new SimpleRefreshTokenProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

        }
    }
}