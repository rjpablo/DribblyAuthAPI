using DribblyAuthAPI.Models.Courts;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace DribblyAuthAPI.Models
{
    public interface IAuthContext
    {
        DbSet<Client> Clients { get; set; }
        DbSet<RefreshToken> RefreshTokens { get; set; }
        DbSet<CourtModel> Courts { get; set; }
        int SaveChanges();
    }

    //This serves as our main connection to our database and Identity tables
    //All Identity tranactions will run on this context
    //You can think about IdentityDbContext class as special version of the traditional “DbContext” Class
    //responsible for handling Identity transactions
    public class AuthContext : IdentityDbContext<IdentityUser>, IAuthContext
    {
        public AuthContext()
            : base("AuthContext")
        {

        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<CourtModel> Courts { get; set; }
    }
}