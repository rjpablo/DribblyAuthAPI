using Dribbly.Authentication.Services;
using Dribbly.Core.Enums.Permissions;
using Dribbly.Core.Exceptions;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Account;
using Dribbly.Model.Accounts;
using Dribbly.Model.Courts;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using Dribbly.Service.Repositories;
using Dribbly.Service.Services.Shared;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Migrations;
using System.Linq;
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

        public AccountsService(IAuthContext context,
            IAccountRepository accountRepo,
            ICourtsRepository courtsRepo,
            IAuthRepository authRepo,
            ISecurityUtility securityUtility,
            IFileService fileService,
            ICommonService commonService,
            IIndexedEntitysRepository indexedEntitysRepo) : base(context.Accounts, context)
        {
            _accountRepo = accountRepo;
            _courtsRepo = courtsRepo;
            _context = context;
            _authRepo = authRepo;
            _fileService = fileService;
            _commonService = commonService;
            _indexedEntitysRepo = indexedEntitysRepo;
            _securityUtility = securityUtility;
            _userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }

        public async Task<AccountViewerModel> GetAccountViewerDataAsync(string userName)
        {
            var account = await GetAccountByUsername(userName);
            if(account == null)
            {
                throw new DribblyObjectNotFoundException($"Account with the username '{userName}' does not exist.");
            }
            return new AccountViewerModel
            {
                Account = account
            };
        }

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
            AccountModel account = _dbSet.Include(u=>u.User).SingleOrDefault(a => a.Id == accountId);
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

        public async Task AddAsync(AccountModel account)
        {
            var user = await _authRepo.FindUserByIdAsync(account.IdentityUserId);
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    account.User = user;
                    Add(account);
                    _context.SetEntityState(account.User, EntityState.Unchanged);
                    await _context.SaveChangesAsync();
                    await _indexedEntitysRepo.Add(_context, new IndexedEntityModel(account));
                    await _commonService.AddUserAccountActivity(UserActivityTypeEnum.CreateAccount, account.Id);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    await _userManager.DeleteAsync(user);
                    throw;
                }
            }
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
                    transaction.Commit();
                    await _indexedEntitysRepo.SetIconUrl(_context, account, photo.Url);
                    await _commonService.AddAccountPhotoActivitiesAsync(UserActivityTypeEnum.AddAccountPhoto, account.Id, photo);
                    await _commonService.AddAccountPhotoActivitiesAsync(UserActivityTypeEnum.SetAccountPrimaryPhoto, account.Id, photo);
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

    }
}