using Dribbly.Authentication.Models;
using Dribbly.Authentication.Models.Auth;
using Dribbly.Core.Enums;
using Dribbly.Core.Exceptions;
using Dribbly.Core.Models;
using Dribbly.Email.Services;
using Dribbly.Identity.Models;
using Dribbly.Model.Account;
using Dribbly.Model.DTO;
using Dribbly.Model.DTO.Account;
using Dribbly.Model.Shared;
using Dribbly.Service.DTO;
using Dribbly.Service.Repositories;
using Dribbly.Service.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    //TODO: Move some logic to a service. Controllers should only pass the call to a service
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private IAuthRepository _repo = null;
        private IEmailService _emailSender = null;
        private IAccountsService _accountService;

        public AccountController(IAuthRepository repo,
            IEmailService emailSender,
            IAccountsService accountService)
        {
            _repo = repo;
            _emailSender = emailSender;
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetAccountByUsername/{userName}")]
        public async Task<PlayerModel> GetAccountByUsername(string userName)
        {
            return await _accountService.GetAccountByUsername(userName);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetPlayerGames/{accountId}")]
        public async Task<IEnumerable<GamePlayer>> GetPlayerGames(long accountId)
        {
            return await _accountService.GetPlayerGames(accountId);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetAccountViewerData/{userName}")]
        public async Task<AccountViewerModel> GetAccountViewerData(string userName)
        {
            return await _accountService.GetAccountViewerDataAsync(userName);
        }

        [HttpGet, Authorize]
        [Route("GetAccountDetailsModal/{accountId}")]
        public async Task<AccountDetailsModalModel> GetAccountDetailsModal(long accountId)
        {
            return await _accountService.GetAccountDetailsModalAsync(accountId);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("GetAccountDropDownSuggestions")]
        public async Task<IEnumerable<AccountsChoicesItemModel>> GetAccountDropDownSuggestions(AccountSearchInputModel input)
        {
            return await _accountService.GetAccountDropDownSuggestions(input);
        }

        #region Account Settings
        [HttpPost]
        [Route("ReplaceEmail")]
        public async Task<IHttpActionResult> ReplaceEmail([FromBody] UpdateEmailInput input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _accountService.ReplaceEmailAsync(input);
            return Ok();
        }

        [HttpGet]
        [Route("GetAccountSettings/{userId}")]
        public async Task<AccountSettingsModel> GetAccountSettings(long userId)
        {
            return await _accountService.GetAccountSettingsAsync(userId);
        }

        [HttpPost]
        [Route("UpdateAccount")]
        public async Task UpdateAccount([FromBody] PlayerModel account)
        {
            await _accountService.UpdateAccountAsync(account);
        }

        [HttpPost]
        [Route("SetHomeCourt/{courtId?}")]
        public async Task SetHomeCourt(long? courtId)
        {
            await _accountService.SetHomeCourt(courtId);
        }

        [HttpPost]
        [Route("SetStatus/{accountId}/{status}")]
        public async Task SetStatus(long accountId, EntityStatusEnum status)
        {
            await _accountService.SetStatus(accountId, status);
        }

        [HttpPost]
        [Route("RemoveFlag/{key}")]
        public async Task RemoveFlag(string key)
        {
            await _accountService.RemoveFlagAsync(key);
        }

        [HttpPost]
        [Route("SetIsPublic/{userId}/{isPublic}")]
        public async Task SetIsPublic(string userId, bool IsPublic)
        {
            await _accountService.SetIsPublic(userId, IsPublic);
        }
        #endregion

        #region Players
        [HttpGet]
        [AllowAnonymous]
        [Route("GetAccountPhotos/{accountId}")]
        public async Task<IEnumerable<MultimediaModel>> GetAccountPhotos(int accountId)
        {
            return await _accountService.GetAccountPhotosAsync(accountId);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetTopPlayers")]
        public async Task<IEnumerable<PlayerStatsViewModel>> GetTopPlayers()
        {
            return await _accountService.GetTopPlayersAsync();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("GetPlayers")]
        public async Task<IEnumerable<PlayerStatsViewModel>> GetPlayers([FromBody] GetPlayersFilterModel filter)
        {
            return await _accountService.GetPlayersAsync(filter);
        }
        #endregion

        #region Account Videos

        [HttpPost]
        [Route("AddAccountVideo/{accountId}/{addToHighlights}")]
        public async Task<VideoModel> AddVideo(long accountId, bool addToHighlights = false)
        {
            HttpFileCollection files = HttpContext.Current.Request.Files;
            if (files.Count > 1)
            {
                throw new BadRequestException("Tried to upload multiple videos at once.");
            }
            else if (files.Count == 0)
            {
                if (HttpContext.Current.Response.ClientDisconnectedToken.IsCancellationRequested)
                {
                    return await Task.FromResult<VideoModel>(null);
                }
                else
                {
                    throw new BadRequestException("Tried to upload a video but no file was received.");
                }
            }

            var result = await Request.Content.ReadAsMultipartAsync();

            var requestJson = await result.Contents[1].ReadAsStringAsync();
            var video = JsonConvert.DeserializeObject<VideoModel>(requestJson);

            return await _accountService.AddVideoAsync(accountId, video, files[0], addToHighlights);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetAccountVideos/{accountId}")]
        public async Task<IEnumerable<VideoModel>> GetAccountVideos(long accountId)
        {
            return await _accountService.GetAccountVideosAsync(accountId);
        }

        [HttpPost]
        [Route("RemoveHighlight/{fileId}")]
        public async Task RemoveHighlight(long fileId)
        {
            await _accountService.RemoveHighlightAsync(fileId);
        }

        [HttpPost]
        [Route("DeleteVideo/{videoId}")]
        public async Task DeleteVideo(long videoId)
        {
            await _accountService.DeleteVideoAsync(videoId);
        }

        #endregion

        #region Account Photos
        [HttpPost]
        [Route("AddAccountPhotos/{accountId}")]
        public async Task<IEnumerable<MultimediaModel>> AddAccountPhotos(long accountId)
        {
            return await _accountService.AddAccountPhotosAsync(accountId);
        }

        [HttpPost]
        [Route("UploadPrimaryPhoto/{accountId}")]
        public async Task<MultimediaModel> UploadPrimaryPhoto(long accountId)
        {
            return await _accountService.UploadPrimaryPhotoAsync(accountId);
        }

        [HttpPost]
        [Route("DeletePhoto/{photoId}/{accountId}")]
        public async Task DeletePhoto(int photoId, int accountId)
        {
            await _accountService.DeletePhoto(photoId, accountId);
        }
        #endregion

        #region Authentication
        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(UserModel userModel)
        {
            //TODO: move logic to a service and use a db transaction
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            (IdentityResult result, ApplicationUser user) result = await _repo.RegisterUser(userModel);

            IHttpActionResult errorResult = GetErrorResult(result.result);

            if (errorResult != null)
            {
                return errorResult;
            }

            await _accountService.AddAsync(new PlayerModel
            {
                IdentityUserId = result.user.Id,
                DateAdded = DateTime.UtcNow,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Username = userModel.UserName
            });

            return Ok();
        }

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            string redirectUri = string.Empty;

            if (error != null)
            {
                return BadRequest(Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            var redirectUriValidationResult = ValidateClientAndRedirectUri(this.Request, ref redirectUri);

            if (!string.IsNullOrWhiteSpace(redirectUriValidationResult))
            {
                return BadRequest(redirectUriValidationResult);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await _repo.FindAsync(new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            redirectUri = string.Format("{0}#/login?external_access_token={1}&provider={2}&haslocalaccount={3}&external_user_name={4}",
                                            redirectUri,
                                            externalLogin.ExternalAccessToken,
                                            externalLogin.LoginProvider,
                                            hasRegistered.ToString(),
                                            externalLogin.UserName);

            //redirectUri = string.Format("{0}#external_access_token={1}&provider={2}&haslocalaccount={3}&external_user_name={4}",
            //                                redirectUri,
            //                                externalLogin.ExternalAccessToken,
            //                                externalLogin.LoginProvider,
            //                                hasRegistered.ToString(),
            //                                externalLogin.UserName);

            return Redirect(redirectUri);

        }

        // Used to test if the client's access token is valid
        [Authorize]
        [Route("VerifyToken")]
        public bool VerifyToken()
        {
            return true;
        }

        [AllowAnonymous]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                return Ok(await _accountService.RegisterExternal(model));
            }
            catch (Exception e)
            {
                if (e is DribblyInvalidOperationException)
                {
                    foreach (var error in ((DribblyInvalidOperationException)e).Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
                return BadRequest(ModelState);
            }

        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ObtainLocalAccessToken")]
        public async Task<IHttpActionResult> ObtainLocalAccessToken(string provider, string externalAccessToken)
        {

            if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(externalAccessToken))
            {
                throw new DribblyInvalidOperationException("Provider or external access token is not sent");
            }

            var verifiedAccessToken = await _accountService.VerifyExternalAccessToken(provider, externalAccessToken);
            if (verifiedAccessToken == null)
            {
                throw new DribblyInvalidOperationException("Invalid Provider or External Access Token");
            }

            ApplicationUser user = await _repo.FindAsync(new UserLoginInfo(provider, verifiedAccessToken.user_id));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                //generate access token response
                var accessTokenResponse = await _accountService.GenerateLocalAccessTokenResponseAsync(user.UserName);
                return Ok(accessTokenResponse);
            }
            else
            {
                JObject tokenResponse = new JObject(
                                        new JProperty("hasRegistered", false),
                                        new JProperty("user_id", verifiedAccessToken.user_id),
                                        new JProperty("given_name", verifiedAccessToken.given_name),
                                        new JProperty("family_name", verifiedAccessToken.family_name),
                                        new JProperty("picture", verifiedAccessToken.picture),
                                        new JProperty("email", verifiedAccessToken.email),
                                        new JProperty("provider", provider),
                                        new JProperty("externalAccessToken", externalAccessToken));
                return Ok(tokenResponse);
            }
        }

        [HttpPost, AllowAnonymous]
        [Route("SendPasswordResetLink")]
        public async Task SendPasswordResetLinkAsync(ForgotPasswordModel input)
        {
            await _repo.SendPasswordResetLinkAsync(input);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ResetPassword")]
        public async Task ResetPassword(ResetPasswordModel input)
        {
            await _repo.ResetPassword(input);
        }

        [HttpPost]
        [Route("ChangePassword")]
        public async Task<bool> ChangePassword(ChangePasswordModel model)
        {
            return await _repo.ChangePassword(model);
        }

        #endregion

        #region Helper Functions
        private string ValidateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriOutput)
        {

            Uri redirectUri;

            var redirectUriString = GetQueryString(Request, "redirect_uri");

            if (string.IsNullOrWhiteSpace(redirectUriString))
            {
                return "redirect_uri is required";
            }

            bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

            if (!validUri)
            {
                return "redirect_uri is invalid";
            }

            var clientId = GetQueryString(Request, "client_id");

            if (string.IsNullOrWhiteSpace(clientId))
            {
                return "client_Id is required";
            }

            var client = _repo.FindClient(clientId);

            if (client == null)
            {
                return string.Format("Client_id '{0}' is not registered in the system.", clientId);
            }

            //TODO: comment out to enforce redirectURI validation
            //if (!string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
            //{
            //    return string.Format("The given URL is not allowed by Client_id '{0}' configuration.", clientId);
            //}

            redirectUriOutput = redirectUri.AbsoluteUri;

            return string.Empty;

        }

        private string GetQueryString(HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();

            if (queryStrings == null) return null;

            var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);

            if (string.IsNullOrEmpty(match.Value)) return null;

            return match.Value;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }
            public string ExternalAccessToken { get; set; }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer) || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name),
                    ExternalAccessToken = identity.FindFirstValue("ExternalAccessToken"),
                };
            }
        }
        #endregion

    }
}
