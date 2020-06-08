using DribblyAuthAPI.Models.Account;
using DribblyAuthAPI.Models.Auth;
using DribblyAuthAPI.Models.Courts;
using DribblyAuthAPI.Models.Games;
using DribblyAuthAPI.Models.Logs;
using DribblyAuthAPI.Models.Shared;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace DribblyAuthAPI.Models
{
    public interface IAuthContext:IDisposable
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

        public static AuthContext Create()
        {
            return new AuthContext();
        }
    }
}