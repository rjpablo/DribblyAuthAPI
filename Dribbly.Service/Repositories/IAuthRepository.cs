using Dribbly.Authentication.Models;
using Dribbly.Authentication.Models.Auth;
using Dribbly.Identity.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
{
    public interface IAuthRepository : IDisposable
    {
        Task<(IdentityResult result, ApplicationUser user)> RegisterUser(UserModel userModel);

        Task<ApplicationUser> FindUser(string userName, string password);

        Task<ApplicationUser> FindUserByName(string userName);

        Task<ApplicationUser> FindAsync(UserLoginInfo loginInfo);

        Task<IdentityResult> CreateAsync(ApplicationUser user);

        Task<IdentityResult> AddLoginAsync(long userId, UserLoginInfo login);

        Client FindClient(string clientId);

        Task<bool> AddRefreshToken(RefreshToken token);

        Task<bool> RemoveRefreshToken(string refreshTokenId);

        Task<bool> RemoveRefreshToken(RefreshToken refreshToken);

        Task<RefreshToken> FindRefreshToken(string refreshTokenId);

        Task<ApplicationUser> FindUserByIdAsync(long userId);

        List<RefreshToken> GetAllRefreshTokens();

        Task SendPasswordResetLinkAsync(ForgotPasswordModel forgotPasswordModel);

        Task<bool> ResetPassword(ResetPasswordModel input);

        Task<bool> ChangePassword(ChangePasswordModel model);
    }
}