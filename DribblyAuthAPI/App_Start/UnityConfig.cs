using DribblyAuthAPI.Models;
using DribblyAuthAPI.Services;
using System.Web.Http;
using Unity;
using Unity.WebApi;

namespace DribblyAuthAPI
{
    public static class UnityConfig
    {
        public static void RegisterComponents(HttpConfiguration config)
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            container.RegisterType<ICourtsService, CourtsService>();
            container.RegisterType<IAuthContext, AuthContext>();
            container.RegisterType<ISettingsService, SettingsService>();

            config.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}