using Dribbly.Core.Helpers;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace DribblyAuthAPI.Models.Auth
{
    public class ApplicationUserManager<TUser> : UserManager<TUser> where TUser : ApplicationUser
    {
        public ApplicationUserManager(IUserStore<TUser> store) : base(store)
        {
        }

        public override async Task<IdentityResult> CreateAsync(TUser user, string password)
        {
            user.Salt = Salt.Generate();
            password = string.Concat(password, user.Salt);
            return await base.CreateAsync(user, password);
        }
    }
}