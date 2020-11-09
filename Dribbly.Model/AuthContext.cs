using Dribbly.Authentication.Models;
using Dribbly.Authentication.Models.Auth;
using Dribbly.Model.Account;
using Dribbly.Model.Accounts;
using Dribbly.Model.Bookings;
using Dribbly.Model.Courts;
using Dribbly.Model.Logs;
using Dribbly.Model.Notifications;
using Dribbly.Model.Shared;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using Dribbly.Model.Games;
using Dribbly.Model.Posts;
using Dribbly.Model.UserActivities;

namespace Dribbly.Model
{
    public interface IAuthContext : IDisposable
    {
        DbSet<AccountModel> Accounts { get; set; }
        DbSet<Client> Clients { get; set; }
        DbSet<RefreshToken> RefreshTokens { get; set; }
        DbSet<CourtModel> Courts { get; set; }
        DbSet<CourtFollowingModel> CourtFollowings { get; set; }
        DbSet<SettingModel> Settings { get; set; }
        DbSet<CourtPhotoModel> CourtPhotos { get; set; }
        DbSet<PhotoModel> Photos { get; set; }
        DbSet<BookingModel> Bookings { get; set; }
        DbSet<GameModel> Games { get; set; }
        DbSet<UserPermissionModel> UserPermissions { get; set; }
        DbSet<AccountPhotoModel> AccountPhotos { get; set; }
        DbSet<CourtVideoModel> CourtVideos { get; set; }
        DbSet<AccountVideoModel> AccountVideos { get; set; }
        DbSet<VideoModel> Videos { get; set; }
        DbSet<ContactModel> Contacts { get; set; }
        DbSet<CourtReviewModel> CourtReivews { get; set; }
        DbSet<PostModel> Posts { get; set; }
        IDbSet<ApplicationUser> Users { get; set; }
        DbEntityEntry Entry(object entity);

        #region User Activites
        DbSet<UserActivityModel> UserActivities { get; set; }
        DbSet<UserPostActivityModel> UserPostActivities { get; set; }
        DbSet<UserAccountActivityModel> UserAccountActivities { get; set; }
        DbSet<AccountPhotoActivityModel> AccountPhotoActivities { get; set; }
        DbSet<AccountVideoActivityModel> AccountVideoActivities { get; set; }
        #endregion

        #region Logs
        DbSet<ClientLogModel> ErrorLogs { get; set; }
        DbSet<ExceptionLog> ExceptionLogs { get; set; }
        #endregion
        #region Notifications
        DbSet<NotificationModel> Notifications { get; set; }
        #endregion

        Database Database { get; }
        Task<int> SaveChangesAsync();
        DbSet<NewBookingNotificationModel> NewBookingNotifications { get; set; }
        int SaveChanges();
    }

    //This serves as our main connection to our database and Identity tables
    //All Identity tranactions will run on this context
    //You can think about IdentityDbContext class as special version of the traditional “DbContext” Class
    //responsible for handling Identity transactions
    public class AuthContext : IdentityDbContext<ApplicationUser>, IAuthContext
    {
        public AuthContext()
            : base("AuthContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<CourtModel> Courts { get; set; }
        public DbSet<CourtFollowingModel> CourtFollowings { get; set; }
        public DbSet<SettingModel> Settings { get; set; }
        public DbSet<PhotoModel> Photos { get; set; }
        public DbSet<CourtPhotoModel> CourtPhotos { get; set; }
        public DbSet<BookingModel> Bookings { get; set; }
        public DbSet<GameModel> Games { get; set; }
        public DbSet<UserPermissionModel> UserPermissions { get; set; }
        public DbSet<AccountPhotoModel> AccountPhotos { get; set; }
        public DbSet<CourtVideoModel> CourtVideos { get; set; }
        public DbSet<AccountVideoModel> AccountVideos { get; set; }
        public DbSet<VideoModel> Videos { get; set; }
        public DbSet<ContactModel> Contacts { get; set; }
        public DbSet<CourtReviewModel> CourtReivews { get; set; }
        public DbSet<PostModel> Posts { get; set; }

        #region User Activites
        public DbSet<UserActivityModel> UserActivities { get; set; }
        public DbSet<UserPostActivityModel> UserPostActivities { get; set; }
        public DbSet<UserAccountActivityModel> UserAccountActivities { get; set; }
        public DbSet<AccountPhotoActivityModel> AccountPhotoActivities { get; set; }
        public DbSet<AccountVideoActivityModel> AccountVideoActivities { get; set; }
        #endregion

        #region Logs
        public DbSet<ClientLogModel> ErrorLogs { get; set; }
        public DbSet<ExceptionLog> ExceptionLogs { get; set; }
        #endregion
        #region Notifications
        public DbSet<NotificationModel> Notifications { get; set; }
        public DbSet<NewBookingNotificationModel> NewBookingNotifications { get; set; }
        #endregion

        public static AuthContext Create()
        {
            return new AuthContext();
        }
    }
}