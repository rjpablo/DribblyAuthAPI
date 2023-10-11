using Microsoft.AspNet.Identity.EntityFramework;

namespace Dribbly.Identity.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, CustomRole,
    long, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public ApplicationDbContext(string contextName)
        : base(contextName)
        {
        }
    }
}
