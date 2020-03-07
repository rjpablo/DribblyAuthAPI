using Dribbly.Core;
using DribblyAuthAPI.Models;
using DribblyAuthAPI.Repositories;
using DribblyAuthAPI.Services;
using System.Web;
using System.Web.Http;
using Unity;
using Unity.Injection;
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

            // Register Core components

            CoreUnityConfig.RegisterComponents(container);

            container.RegisterType<ICourtsService, CourtsService>();
            container.RegisterType<IGamesService, GamesService>();
            container.RegisterType<IAuthContext, AuthContext>();
            container.RegisterType<ISettingsService, SettingsService>();
            container.RegisterType<IFileService, FileService>();
            container.RegisterType<IAuthRepository, AuthRepository>();

            config.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}