using Dribbly.Core;
using Dribbly.Email.Models;
using DribblyAuthAPI.Models;
using DribblyAuthAPI.Repositories;
using DribblyAuthAPI.Services;
using System.Web;
using System.Web.Http;
using System.Configuration;
using Unity;
using Unity.Injection;
using Unity.WebApi;
using System;
using Dribbly.Email.Services;

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

            container.RegisterType<IAccountsService, AccountsService>();
            container.RegisterType<ICourtsService, CourtsService>();
            container.RegisterType<IGamesService, GamesService>();
            container.RegisterType<IAuthContext, AuthContext>();
            container.RegisterType<ISettingsService, SettingsService>();
            container.RegisterType<IFileService, FileService>();
            container.RegisterType<IAuthRepository, AuthRepository>();
            container.RegisterType<ICourtsRepository, CourtsRepository>();
            container.RegisterType<IAccountRepository, AccountRepository>();
            container.RegisterType<IEmailConfiguration, EmailConfiguration>();
            container.RegisterType<IEmailService, EmailService>();

            config.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}