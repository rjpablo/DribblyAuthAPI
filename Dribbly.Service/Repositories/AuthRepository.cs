using Dribbly.Email.Models;
using Dribbly.Email.Services;
using Dribbly.Service.Models;
using Dribbly.Service.Models.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Dribbly.Service.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private IAuthContext _ctx;
        private ApplicationUserManager _userManager;
        private IEmailService _emailSender = null;

        public AuthRepository(IEmailService emailSender, IAuthContext context)
        {
            _ctx = context;
            _userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            _emailSender = emailSender;
        }

        public async Task<(IdentityResult result, ApplicationUser user)> RegisterUser(UserModel userModel)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = userModel.UserName,
                Email = userModel.Email
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);

            return (result, user);
        }

        public async Task<bool> ResetPassword(ResetPasswordModel input)
        {
            var user = await _userManager.FindByEmailAsync(input.Email);
            if (user == null)
                return false;
            input.Token = input.Token.Replace(" ", "+"); // +'s in the token are replaced with a space when token is sent using query strings.
            IdentityResult resetPassResult = await _userManager.ResetPasswordAsync(user.Id, input.Token, input.Password);

            if (!resetPassResult.Succeeded)
            {
                throw new Exception(resetPassResult.Errors.ElementAt(0));
            }

            return true;

        }

        public async Task<bool> ChangePassword(ChangePasswordModel model)
        {
            ApplicationUser user = _userManager.FindById(model.Userid);

            IdentityResult result = await _userManager.ChangePasswordAsync(model.Userid,
                string.Concat(model.CurrentPassword, user.Salt), string.Concat(model.NewPassword, user.Salt));

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.ElementAt(0));
            }

            return true;
        }

        public async Task<ApplicationUser> FindUser(string userName, string password)
        {
            ApplicationUser user = await _userManager.FindAsync(userName, password);

            return user;
        }

        public async Task<ApplicationUser> FindUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser> FindUserByName(string userName)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(userName);

            return user;
        }

        public async Task<ApplicationUser> FindAsync(UserLoginInfo loginInfo)
        {
            ApplicationUser user = await _userManager.FindAsync(loginInfo);

            return user;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user)
        {
            var result = await _userManager.CreateAsync(user);

            return result;
        }

        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            var result = await _userManager.AddLoginAsync(userId, login);

            return result;
        }

        public Client FindClient(string clientId)
        {
            var client = _ctx.Clients.Find(clientId);

            return client;
        }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {

            var existingToken = _ctx.RefreshTokens.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId).SingleOrDefault();

            if (existingToken != null)
            {
                var result = await RemoveRefreshToken(existingToken);
            }

            _ctx.RefreshTokens.Add(token);

            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            if (refreshToken != null)
            {
                _ctx.RefreshTokens.Remove(refreshToken);
                return await _ctx.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            _ctx.RefreshTokens.Remove(refreshToken);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            return refreshToken;
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
            return _ctx.RefreshTokens.ToList();
        }

        public async Task SendPasswordResetLinkAsync(ForgotPasswordModel forgotPasswordModel)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
            string webClientHostName = ConfigurationManager.AppSettings["WEB_CLIENT_HOSTNAME"];
            if (user == null)
                return;

            string token = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
            var message = new EmailMessage(new string[] { forgotPasswordModel.Email }, "Reset password",
                string.Format("Please click <a href=\"{0}passwordreset?token={1}&email={2}\">here</a> to reset your password.",
                webClientHostName, token, forgotPasswordModel.Email));
            await _emailSender.SendEmailAsync(message);

            return;
        }

        public void Dispose()
        {

        }
    }
}