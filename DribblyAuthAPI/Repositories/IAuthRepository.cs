using DribblyAuthAPI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DribblyAuthAPI.Repositories
{
    public interface IAuthRepository: IDisposable
    {
        Task<IdentityResult> RegisterUser(UserModel userModel);

        Task<IdentityUser> FindUser(string userName, string password);

        Task<IdentityUser> FindAsync(UserLoginInfo loginInfo);

        Task<IdentityResult> CreateAsync(IdentityUser user);

        Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login);

        Client FindClient(string clientId);

        Task<bool> AddRefreshToken(RefreshToken token);

        Task<bool> RemoveRefreshToken(string refreshTokenId);

        Task<bool> RemoveRefreshToken(RefreshToken refreshToken);

        Task<RefreshToken> FindRefreshToken(string refreshTokenId);

        List<RefreshToken> GetAllRefreshTokens();

        void Dispose();
    }
}