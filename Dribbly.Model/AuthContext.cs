using Dribbly.Authentication.Models;
using Dribbly.Authentication.Models.Auth;
using Dribbly.Model.Account;
using Dribbly.Model.Accounts;
using Dribbly.Model.Courts;
using Dribbly.Model.Games;
using Dribbly.Model.Logs;
using Dribbly.Model.Shared;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Dribbly.Model
{
    public interface IAuthContext : IDisposable
    {
        DbSet<AccountModel> Accounts { get; set; }
        DbSet<Client> Clients { get; set; }
        DbSet<RefreshToken> RefreshTokens { get; set; }
        DbSet<CourtModel> Courts { get; set; }
        DbSet<SettingModel> Settings { get; set; }
        DbSet<CourtPhotoModel> CourtPhotos { get; set; }
        DbSet<PhotoModel> Photos { get; set; }
        DbSet<EventModel> Events { get; set; }
        DbSet<GameModel> Games { get; set; }
        DbSet<ClientLogModel> ErrorLogs { get; set; }
        DbSet<UserPermissionModel> UserPermissions { get; set; }
        DbSet<AccountPhotoModel> AccountPhotos { get; set; }

        Database Database { get; }
        Task<int> SaveChangesAsync();
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
        public DbSet<SettingModel> Settings { get; set; }
        public DbSet<PhotoModel> Photos { get; set; }
        public DbSet<CourtPhotoModel> CourtPhotos { get; set; }
        public DbSet<EventModel> Events { get; set; }
        public DbSet<GameModel> Games { get; set; }
        public DbSet<ClientLogModel> ErrorLogs { get; set; }
        public DbSet<UserPermissionModel> UserPermissions { get; set; }
        public DbSet<AccountPhotoModel> AccountPhotos { get; set; }

        public static AuthContext Create()
        {
            return new AuthContext();
        }
    }
}