using Dribbly.Authentication.Models;
using Dribbly.Authentication.Services;
using Dribbly.Core.Enums.Permissions;
using Dribbly.Core.Exceptions;
using Dribbly.Core.Utilities;
using Dribbly.Email.Services;
using Dribbly.Identity.Models;
using Dribbly.Model;
using Dribbly.Model.Account;
using Dribbly.Model.Accounts;
using Dribbly.Model.Courts;
using Dribbly.Model.DTO;
using Dribbly.Model.DTO.Account;
using Dribbly.Model.Entities;
using Dribbly.Model.Enums;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using Dribbly.Service.Repositories;
using Dribbly.Service.Services.Shared;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Dribbly.Service.Services
{
    public class AccountsService : BaseEntityService<AccountModel>, IAccountsService
    {
        IAccountRepository _accountRepo;
        private readonly ICourtsRepository _courtsRepo;
        IAuthContext _context;
        IAuthRepository _authRepo;
        IFileService _fileService;
        private readonly ICommonService _commonService;
        private readonly IIndexedEntitysRepository _indexedEntitysRepo;
        ISecurityUtility _securityUtility;
        private ApplicationUserManager _userManager;
        private OAuthBearerAuthenticationOptions _oAuthBearerOptions;

        public AccountsService(IAuthContext context,
            IAccountRepository accountRepo,
            ICourtsRepository courtsRepo,
            IAuthRepository authRepo,
            ISecurityUtility securityUtility,
            IFileService fileService,
            IIndexedEntitysRepository indexedEntitysRepo,
            IEmailService emailSender,
            OAuthBearerAuthenticationOptions oAuthBearerAuthenticationOptions) : base(context.Accounts, context)
        {
            _courtsRepo = courtsRepo;
            _context = context;
            _authRepo = new AuthRepository(emailSender, context);
            _accountRepo = new AccountRepository(context, _authRepo);
            _fileService = fileService;
            _commonService = new CommonService(context, securityUtility);
            _indexedEntitysRepo = indexedEntitysRepo;
            _securityUtility = securityUtility;
            _userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            _oAuthBearerOptions = oAuthBearerAuthenticationOptions;
        }

        public async Task<AccountViewerModel> GetAccountViewerDataAsync(string userName)
        {
            var account = await GetAccountByUsername(userName);
            if (account == null)
            {
                throw new DribblyObjectNotFoundException($"Account with the username '{userName}' does not exist.");
            }
            var stats = await _context.PlayerStats.SingleOrDefaultAsync(s => s.AccountId == account.Id);
            return new AccountViewerModel
            {
                Account = account,
                Stats = stats
            };
        }

        #region GetPlayers

        public async Task<IEnumerable<PlayerStatsViewModel>> GetPlayersAsync(GetPlayersFilterModel filter)
        {
            var query = _context.PlayerStats
                .Include(s => s.Account.User).Include(s => s.Account.ProfilePhoto)
                .Where(s => !filter.CourtIds.Any() || (s.Account.HomeCourtId.HasValue && filter.CourtIds.Contains(s.Account.HomeCourtId.Value)));
            query = ApplySortingAndPaging(query, filter);
            var players = await query.ToListAsync();
            return players.Select(s => new PlayerStatsViewModel(s));
        }

        private IQueryable<PlayerStatsModel> ApplySortingAndPaging(IQueryable<PlayerStatsModel> query, GetPlayersFilterModel filter)
        {
            IOrderedQueryable<PlayerStatsModel> ordered = null;
            bool isAscending = filter.SortDirection == SortDirectionEnum.Ascending;
            switch (filter.SortBy)
            {
                case StatEnum.PPG:
                    ordered = isAscending ? query.OrderBy(s => s.PPG) : query.OrderByDescending(s => s.PPG);
                    break;
                case StatEnum.RPG:
                    ordered = isAscending ? query.OrderBy(s => s.RPG) : query.OrderByDescending(s => s.RPG);
                    break;
                case StatEnum.APG:
                    ordered = isAscending ? query.OrderBy(s => s.APG) : query.OrderByDescending(s => s.APG);
                    break;
                case StatEnum.FGP:
                    ordered = isAscending ? query.OrderBy(s => s.FGP) : query.OrderByDescending(s => s.FGP);
                    break;
                case StatEnum.BPG:
                    ordered = isAscending ? query.OrderBy(s => s.BPG) : query.OrderByDescending(s => s.BPG);
                    break;
                case StatEnum.TPG:
                    ordered = isAscending ? query.OrderBy(s => s.TPG) : query.OrderByDescending(s => s.TPG);
                    break;
                case StatEnum.SPG:
                    ordered = isAscending ? query.OrderBy(s => s.SPG) : query.OrderByDescending(s => s.SPG);
                    break;
                case StatEnum.ThreePP:
                    ordered = isAscending ? query.OrderBy(s => s.ThreePP) : query.OrderByDescending(s => s.ThreePP);
                    break;
                default:
                    ordered = isAscending ? query.OrderBy(s => s.OverallScore) : query.OrderByDescending(s => s.OverallScore);
                    break;
            }
            return ordered.Skip(filter.PageSize * (filter.Page - 1))
                .Take(filter.PageSize);
        }
        #endregion

        public Task<AccountModel> GetAccountByUsername(string userName)
        {
            return _accountRepo.GetAccountByUsername(userName);
        }

        public async Task<AccountSettingsModel> GetAccountSettingsAsync(long userId)
        {
            AccountSettingsModel settings = new AccountSettingsModel();
            AccountModel account = await _accountRepo.GetAccountByIdentityId(userId);
            if (account == null)
            {
                throw new DribblyForbiddenException(string.Format("Did not find an account with User ID {0}", userId),
                        friendlyMessageKey: "app.Error_CouldNotRetrieveAccountSettingsAccountNotFound");
            }
            else
            {
                if (_securityUtility.IsCurrentUser(account.IdentityUserId) ||
                    AuthenticationService.HasPermission(AccountPermission.UpdateNotOwned))
                {
                    settings.IsPublic = account.IsPublic;
                }
                else
                {
                    throw new DribblyForbiddenException("Authorization failed when attempting to retrieve account settings.");
                }
            }

            return await Task.FromResult(settings);
        }

        public async Task<IEnumerable<AccountsChoicesItemModel>> GetAccountDropDownSuggestions(AccountSearchInputModel input)
        {
            List<AccountModel> accounts = await _accountRepo.SearchAccounts(input).ToListAsync();
            return accounts.Select(a => new AccountsChoicesItemModel(a));
        }

        #region Account Updates

        public async Task ReplaceEmailAsync(UpdateEmailInput input)
        {
            var userId = _securityUtility.GetUserId().Value;
            var emialUser = _userManager.FindByEmail(input.NewEmail);
            if(emialUser != null)
            {
                throw new DribblyInvalidOperationException("Email already taken", friendlyMessage: $"{input.NewEmail} is already in use.");
            }
            await _userManager.SetEmailAsync(userId, input.NewEmail);
        }

        public async Task<AccountDetailsModalModel> GetAccountDetailsModalAsync(long accountId)
        {
            var account = await _accountRepo.GetAccountById(accountId);
            ChoiceItemModel<long> homeCourtChoice = await _commonService.GetChoiceItemModelAsync
                (account.HomeCourtId, EntityTypeEnum.Court);
            return new AccountDetailsModalModel
            {
                Account = account,
                HomeCourtChoice = homeCourtChoice
            };
        }

        public async Task UpdateAccountAsync(AccountModel account)
        {
            _dbSet.AddOrUpdate(account);
            await _indexedEntitysRepo.Update(_context, account);
            await _context.SaveChangesAsync();
        }

        public async Task SetStatus(long accountId, EntityStatusEnum status)
        {
            AccountModel account = _dbSet.Include(u => u.User).SingleOrDefault(a => a.Id == accountId);
            if (account == null)
            {
                throw new DribblyObjectNotFoundException
                    (string.Format("Did not find an account with Account ID {0}", accountId));
            }
            else
            {
                if (_securityUtility.IsCurrentUser(account.IdentityUserId) ||
                    AuthenticationService.HasPermission(AccountPermission.UpdateNotOwned))
                {
                    // TODO: Remove personal info when deleting an account
                    account.EntityStatus = status;
                    await _indexedEntitysRepo.Update(_context, account);
                    await _context.SaveChangesAsync();
                    if (status == EntityStatusEnum.Active)
                    {
                        await _commonService.AddUserAccountActivity(UserActivityTypeEnum.ReactivateAccount, account.Id);
                    }
                    else if (status == EntityStatusEnum.Inactive)
                    {
                        await _commonService.AddUserAccountActivity(UserActivityTypeEnum.DeactivateAccount, account.Id);
                    }
                    else if (status == EntityStatusEnum.Deleted)
                    {
                        await _commonService.AddUserAccountActivity(UserActivityTypeEnum.DeleteAccount, account.Id);
                    }
                    else
                    {
                        // TODO: Log warning: Account status activity not recorded
                    }
                }
                else
                {
                    throw new DribblyForbiddenException("Authorization failed when attempting to set account status.");
                }
            }
        }

        #region Register External

        public async Task<JObject> RegisterExternal(RegisterExternalBindingModel model)
        {
            JObject accessTokenResponse = null;
            var verifiedAccessToken = await VerifyExternalAccessToken(model.Provider, model.ExternalAccessToken);
            if (verifiedAccessToken == null || verifiedAccessToken.user_id != model.UserId)
            {
                throw new DribblyInvalidOperationException("Invalid Provider or External Access Token", friendlyMessage: "Invalid Provider or External Access Token");
            }

            ApplicationUser user = await _authRepo.FindAsync(new UserLoginInfo(model.Provider, verifiedAccessToken.user_id));
            bool hasRegistered = user != null;
            if (hasRegistered)
            {
                accessTokenResponse = await GenerateLocalAccessTokenResponseAsync(model.UserName);
                return accessTokenResponse;
            }

            using (var tx = _context.Database.BeginTransaction())
            {
                try
                {
                    user = new ApplicationUser() { UserName = model.UserName, Email = model.Email, EmailConfirmed = true };
                    IdentityResult result = await _authRepo.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        throw new DribblyInvalidOperationException("Registration failed.", friendlyMessage: "Registration failed. Please try again.", errors: result.Errors);
                    }

                    var info = new ExternalLoginInfo()
                    {
                        DefaultUserName = model.UserName,
                        Login = new UserLoginInfo(model.Provider, verifiedAccessToken.user_id)
                    };

                    result = await _authRepo.AddLoginAsync(user.Id, info.Login);
                    if (!result.Succeeded)
                    {
                        throw new DribblyInvalidOperationException("Registration failed.", friendlyMessage: "Registration failed. Please try again.", errors: result.Errors);
                    }

                    await _AddAsync(new AccountModel
                    {
                        IdentityUserId = user.Id,
                        DateAdded = DateTime.UtcNow,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        EntityStatus = EntityStatusEnum.Active
                    });

                    //generate access token response
                    accessTokenResponse = await GenerateLocalAccessTokenResponseAsync(model.UserName);
                    tx.Commit();
                    return accessTokenResponse;
                }
                catch (Exception)
                {
                    tx.Rollback();
                    await _userManager.DeleteAsync(user);
                    throw;
                }
            }
        }

        public async Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken)
        {
            ParsedExternalAccessToken parsedToken = null;

            var verifyTokenEndPoint = "";

            if (provider == "Facebook")
            {
                //You can get it from here: https://developers.facebook.com/tools/accesstoken/
                //More about debug_tokn here: http://stackoverflow.com/questions/16641083/how-does-one-get-the-app-access-token-for-debug-token-inspection-on-facebook

                // App Name: FreeHoops Test 2
                var appToken = "271234698173280|Fp8OFeqRL4aCEAApmlvlRSkebOw";
                verifyTokenEndPoint = string.Format("https://graph.facebook.com/debug_token?input_token={0}&access_token={1}", accessToken, appToken);
            }
            else if (provider == "Google")
            {
                verifyTokenEndPoint = string.Format("https://oauth2.googleapis.com/tokeninfo?id_token={0}", accessToken);
            }
            else
            {
                return null;
            }

            var client = new HttpClient();
            var uri = new Uri(verifyTokenEndPoint);
            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                JObject jObj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                parsedToken = new ParsedExternalAccessToken();
                if (provider == "Google")
                {
                    parsedToken.user_id = jObj.Value<string>("sub");
                    parsedToken.app_id = jObj.Value<string>("aud");
                    parsedToken.given_name = jObj.Value<string>("given_name");
                    parsedToken.family_name = jObj.Value<string>("family_name");
                    parsedToken.email = jObj.Value<string>("email");
                    parsedToken.picture = jObj.Value<string>("picture");
                }

            }

            return parsedToken;
        }
        public async Task<JObject> GenerateLocalAccessTokenResponseAsync(string userName)
        {
            var tokenExpiration = TimeSpan.FromMinutes(30);
            var account = await GetAccountByUsername(userName);

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim("userId", account.IdentityUserId.ToString()));
            identity.AddClaim(new Claim("role", "user"));

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);                       
            var accessToken = _oAuthBearerOptions.AccessTokenFormat.Protect(ticket);

            JObject tokenResponse = new JObject(
                                        new JProperty("hasRegistered", true),
                                        new JProperty("userName", userName),
                                        new JProperty("userId", account.IdentityUserId),
                                        new JProperty("access_token", accessToken),
                                        new JProperty("token_type", "bearer"),
                                        new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
                                        new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                                        new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
        );

            return tokenResponse;
        }

        #endregion

        public async Task AddAsync(AccountModel account)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _AddAsync(account);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private async Task _AddAsync(AccountModel account)
        {
            Add(account);
            await _context.SaveChangesAsync();
            var user = await _authRepo.FindUserByIdAsync(account.IdentityUserId);
            account.User = user;
            _context.SetEntityState(account.User, EntityState.Unchanged);
            _context.SetEntityState(account.User.Logins.First(), EntityState.Unchanged);
            var entity = new IndexedEntityModel(account);
            await _indexedEntitysRepo.Add(_context, entity, entity.AdditionalData);
            await _commonService.AddUserAccountActivity(UserActivityTypeEnum.CreateAccount, account.Id);
        }

        public async Task SetIsPublic(string userId, bool IsPublic)
        {
            AccountModel account = await _context.Accounts.SingleOrDefaultAsync(a => a.IdentityUserId.ToString() == userId);
            if (account == null)
            {
                throw new DribblyObjectNotFoundException
                    (string.Format("Did not find an account with User ID {0}", userId));
            }
            else
            {
                if (_securityUtility.IsCurrentUser(account.IdentityUserId) ||
                    AuthenticationService.HasPermission(AccountPermission.UpdateNotOwned))
                {
                    // TODO: Remove personal info when deleting an account
                    account.IsPublic = IsPublic;
                    await _context.SaveChangesAsync();
                    if (IsPublic)
                    {
                        await _commonService.AddUserAccountActivity(UserActivityTypeEnum.MakeAccountPublic, account.Id);
                    }
                    else
                    {
                        await _commonService.AddUserAccountActivity(UserActivityTypeEnum.MakeAccountPrivate, account.Id);
                    }
                }
                else
                {
                    throw new DribblyForbiddenException("Authorization failed when attempting to set IsPublic property.");
                }
            }
        }

        #endregion

        #region Photos

        public async Task<IEnumerable<PhotoModel>> AddAccountPhotosAsync(long accountId)
        {
            HttpFileCollection files = HttpContext.Current.Request.Files;
            AccountModel account = GetById(accountId);
            List<PhotoModel> photos = new List<PhotoModel>();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        photos.Add(await AddPhoto(account, files[i]));
                    }
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    await _commonService.AddAccountPhotoActivitiesAsync(UserActivityTypeEnum.AddAccountPhoto, account.Id, photos.ToArray());
                    return photos;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public async Task<IEnumerable<PhotoModel>> GetAccountPhotosAsync(int accountId)
        {
            return await _context.AccountPhotos.Include(p => p.Photo)
                .Where(p => p.AccountId == accountId && p.Photo.DateDeleted == null)
                .Select(p => p.Photo)
                .ToListAsync();
        }

        public async Task DeletePhoto(int photoId, int accountId)
        {
            AccountPhotoModel accountPhoto = await _context.AccountPhotos.Include(p2 => p2.Photo)
                .SingleOrDefaultAsync(p => p.AccountId == accountId && p.PhotoId == photoId);
            if (accountPhoto == null)
            {
                throw new ObjectNotFoundException
                    (string.Format("Did not find a photo with the ID {0} that is associated to Account # {1}", photoId, accountId));
            }
            else
            {
                if (_securityUtility.IsCurrentUser(accountPhoto.Photo.UploadedById) ||
                    AuthenticationService.HasPermission(AccountPermission.DeletePhotoNotOwned))
                {
                    accountPhoto.Photo.DateDeleted = DateTime.UtcNow;
                    _context.AccountPhotos.AddOrUpdate(accountPhoto);
                    _context.SaveChanges();
                    await _commonService.AddAccountPhotoActivitiesAsync(UserActivityTypeEnum.DeleteAccountPhoto, accountId, accountPhoto.Photo);
                    // TODO: delete file from server
                }
                else
                {
                    throw new UnauthorizedAccessException("Authorization failed when attempting to delete account photo.");
                }
            }
        }

        public async Task<PhotoModel> UploadPrimaryPhotoAsync(long accountId)
        {
            AccountModel account = GetById(accountId);
            long? currentUserId = _securityUtility.GetUserId();

            if ((currentUserId != account.IdentityUserId) && !AuthenticationService.HasPermission(AccountPermission.UpdatePhotoNotOwned))
            {
                throw new UnauthorizedAccessException("Authorization failed when attempting to update account primary photo.");
            }

            HttpFileCollection files = HttpContext.Current.Request.Files;
            string uploadPath = _fileService.Upload(files[0], "accountPhotos/");

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    PhotoModel photo = await AddPhoto(account, files[0]);
                    account.ProfilePhotoId = photo.Id;
                    Update(account);
                    await _context.SaveChangesAsync();
                    await _indexedEntitysRepo.SetIconUrl(_context, account, photo.Url);
                    await _commonService.AddAccountPhotoActivitiesAsync(UserActivityTypeEnum.AddAccountPhoto, account.Id, photo);
                    await _commonService.AddAccountPhotoActivitiesAsync(UserActivityTypeEnum.SetAccountPrimaryPhoto, account.Id, photo);
                    transaction.Commit();
                    return photo;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        private async Task<PhotoModel> AddPhoto(AccountModel account, HttpPostedFile file)
        {
            string uploadPath = _fileService.Upload(file, "accountPhotos/");

            PhotoModel photo = new PhotoModel
            {
                Url = uploadPath,
                UploadedById = _securityUtility.GetAccountId().Value,
                DateAdded = DateTime.UtcNow
            };
            _context.Photos.Add(photo);
            await _context.SaveChangesAsync();

            _context.AccountPhotos.Add(new AccountPhotoModel
            {
                PhotoId = photo.Id,
                AccountId = account.Id
            });
            await _context.SaveChangesAsync();

            return photo;
        }

        #endregion

        #region Account Videos

        public async Task<IEnumerable<VideoModel>> GetAccountVideosAsync(long accountId)
        {
            AccountModel account = await _dbSet.FirstOrDefaultAsync(c => c.Id == accountId);

            if (account == null)
            {
                throw new ObjectNotFoundException("No account was found with id " + accountId.ToString());
            }

            return _context.AccountVideos.Include(v => v.Account).Where(v => v.AccountId == accountId).Select(v => v.Video)
                .OrderByDescending(v => v.DateAdded);
        }

        public async Task<VideoModel> AddVideoAsync(long accountId, VideoModel video, HttpPostedFile file)
        {
            AccountModel account = await GetByIdAsync(accountId);

            if (account == null)
            {
                throw new ObjectNotFoundException("No account was found with id " + accountId.ToString());
            }

            if (account.IdentityUserId == _securityUtility.GetUserId() || AuthenticationService.HasPermission(AccountPermission.AddVideoNotOwned))
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        AddAccountVideo(accountId, video, file);
                        _context.SaveChanges();
                        transaction.Commit();
                        await _commonService.AddAccountVideoActivitiesAsync(UserActivityTypeEnum.AddAccountVideo, account.Id, video);
                        return video;
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw e;
                    }
                }
            }
            else
            {
                throw new UnauthorizedAccessException
                    (string.Format("Authorization failed when trying to upload a video to account with Account ID {0}", accountId));
            }
        }

        private VideoModel AddAccountVideo(long accountId, VideoModel video, HttpPostedFile file)
        {
            string uploadPath = _fileService.Upload(file, "video/");
            video.Src = uploadPath;
            video.AddedBy = _securityUtility.GetAccountId().Value;
            video.DateAdded = DateTime.UtcNow;
            video.Size = file.ContentLength;
            video.Type = file.ContentType;

            _context.Videos.Add(video);
            _context.AccountVideos.Add(new AccountVideoModel
            {
                AccountId = accountId,
                VideoId = video.Id
            });

            return video;
        }

        #endregion

        public async Task<IEnumerable<PlayerStatsViewModel>> GetTopPlayersAsync()
        {
            var result = await _context.PlayerStats.Include(s => s.Account.User).Include(s => s.Account.ProfilePhoto)
                .OrderByDescending(s => s.OverallScore)
                .Take(10)
                .ToListAsync();

            return result.Select(s => new PlayerStatsViewModel(s));
        }
    }

    public interface IAccountsService
    {
        Task<AccountModel> GetAccountByUsername(string userName);

        Task<AccountDetailsModalModel> GetAccountDetailsModalAsync(long accountId);

        Task<AccountViewerModel> GetAccountViewerDataAsync(string userName);

        Task AddAsync(AccountModel account);

        Task<IEnumerable<PhotoModel>> AddAccountPhotosAsync(long accountId);

        Task ReplaceEmailAsync(UpdateEmailInput input);

        Task<AccountSettingsModel> GetAccountSettingsAsync(long userId);

        Task<PhotoModel> UploadPrimaryPhotoAsync(long accountId);

        Task UpdateAccountAsync(AccountModel account);

        Task<IEnumerable<PhotoModel>> GetAccountPhotosAsync(int accountId);

        Task DeletePhoto(int photoId, int accountId);

        Task<IEnumerable<VideoModel>> GetAccountVideosAsync(long accountId);

        Task<IEnumerable<PlayerStatsViewModel>> GetPlayersAsync(GetPlayersFilterModel filter);

        Task<VideoModel> AddVideoAsync(long accountId, VideoModel video, HttpPostedFile file);

        Task<IEnumerable<AccountsChoicesItemModel>> GetAccountDropDownSuggestions(AccountSearchInputModel input);

        Task SetStatus(long accountId, EntityStatusEnum status);

        Task SetIsPublic(string userId, bool IsPublic);

        Task<IEnumerable<PlayerStatsViewModel>> GetTopPlayersAsync();
        Task<JObject> RegisterExternal(RegisterExternalBindingModel model);
        Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken);
        Task<JObject> GenerateLocalAccessTokenResponseAsync(string userName);
    }
}