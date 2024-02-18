using Dribbly.Chat.Data;
using Dribbly.Chat.Services;
using Dribbly.Core;
using Dribbly.Email.Models;
using Dribbly.Email.Services;
using Dribbly.Model;
using Dribbly.Service.Repositories;
using Dribbly.Service.Services;
using Dribbly.Service.Services.Shared;
using Dribbly.Test;
using DribblyAuthAPI.API;
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

            // Register Core components

            CoreUnityConfig.RegisterComponents(container);

            container.RegisterType<IAccountsService, AccountsService>();
            container.RegisterType<ICourtsService, CourtsService>();
            container.RegisterType<IGamesService, GamesService>();
            container.RegisterType<IBookingsService, BookingsService>();
            container.RegisterType<IContactsService, ContactsService>();
            container.RegisterType<IAuthContext, AuthContext>();
            container.RegisterType<ISettingsService, SettingsService>();
            container.RegisterType<IFileService, FileService>();
            container.RegisterType<IAuthRepository, AuthRepository>();
            container.RegisterType<ICourtsRepository, CourtsRepository>();
            container.RegisterType<IAccountRepository, AccountRepository>();
            container.RegisterType<IEmailConfiguration, EmailConfiguration>();
            container.RegisterType<IEmailService, EmailService>();
            container.RegisterType<ILogsService, LogsService>();
            container.RegisterType<IPermissionsService, PermissionsService>();
            container.RegisterType<IPermissionsRepository, PermissionsRepository>();
            container.RegisterType<INotificationsRepository, NotificationsRepository>();
            container.RegisterType<INotificationsService, NotificationsService>();
            container.RegisterType<IPostsService, PostsService>();
            container.RegisterType<IBlogsService, BlogsService>();
            container.RegisterType<IEventsService, EventsService>();
            container.RegisterType<ICommonService, CommonService>();
            container.RegisterType<IIndexedEntitysRepository, IndexedEntitysRepository>();
            container.RegisterType<ITeamsRepository, TeamsRepository>();
            container.RegisterType<IShotsRepository, ShotsRepository>();
            container.RegisterType<ITeamsService, TeamsService>();
            container.RegisterType<ILeaguesRepository, LeaguesRepository>();
            container.RegisterType<ILeaguesService, LeaguesService>();
            container.RegisterType<ITournamentsRepository, TournamentsRepository>();
            container.RegisterType<ITournamentsService, TournamentsService>();
            container.RegisterType<IGameEventsService, GameEventsService>();
            container.RegisterType<IDribblyChatService, DribblyChatService>();
            container.RegisterType<IMultimediaService, MultimediaService>();
            container.RegisterType<IGroupsService, GroupsService>();
            container.RegisterType<ICommentsService, CommentsService>();
            container.RegisterType<IChatDbContext, AuthContext>();
            container.RegisterInstance(Startup.OAuthBearerOptions);

            TestsConfig.RegisterComponents(container);

            config.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}