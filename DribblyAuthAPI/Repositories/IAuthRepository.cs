using DribblyAuthAPI.Models;
using DribblyAuthAPI.Models.Auth;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DribblyAuthAPI.Repositories
{
    public interface IAuthRepository: IDisposable
    {
        Task<IdentityResult> RegisterUser(UserModel userModel);

        Task<ApplicationUser> FindUser(string userName, string password);

        Task<ApplicationUser> FindAsync(UserLoginInfo loginInfo);

        Task<IdentityResult> CreateAsync(ApplicationUser user);

        Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login);

        Client FindClient(string clientId);

        Task<bool> AddRefreshToken(RefreshToken token);

        Task<bool> RemoveRefreshToken(string refreshTokenId);

        Task<bool> RemoveRefreshToken(RefreshToken refreshToken);

        Task<RefreshToken> FindRefreshToken(string refreshTokenId);

        List<RefreshToken> GetAllRefreshTokens();

        Task SendPasswordResetLinkAsync(ForgotPasswordModel forgotPasswordModel);
    }
}