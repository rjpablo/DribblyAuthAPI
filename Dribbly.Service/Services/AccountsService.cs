using Dribbly.Authentication.Services;
using Dribbly.Core.Enums.Permissions;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Account;
using Dribbly.Model.Accounts;
using Dribbly.Model.Courts;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using Dribbly.Service.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace Dribbly.Service.Services
{
    public class AccountsService : BaseEntityService<AccountModel>, IAccountsService
    {
        IAccountRepository _accountRepo;
        IAuthContext _context;
        IAuthRepository _authRepo;
        IFileService _fileService;
        ISecurityUtility _securityUtility;

        public AccountsService(IAuthContext context,
            IAccountRepository accountRepo,
            IAuthRepository authRepo,
            ISecurityUtility securityUtility,
            IFileService fileService) : base(context.Accounts)
        {
            _accountRepo = accountRepo;
            _context = context;
            _authRepo = authRepo;
            _fileService = fileService;
            _securityUtility = securityUtility;
        }

        public Task<AccountModel> GetAccountByUsername(string userName)
        {
            return _accountRepo.GetAccountByUsername(userName);
        }

        public async Task<AccountSettingsModel> GetAccountSettingsAsync(string userId)
        {
            AccountSettingsModel settings = new AccountSettingsModel();

            AccountModel account = await _accountRepo.GetAccountById(userId);
            if (account == null)
            {
                throw new ObjectNotFoundException
                    (string.Format("Did not find an account with User ID {0}", userId));
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
                    throw new UnauthorizedAccessException("Authorization failed when attempting to retrieve account settings.");
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

        public async Task SetStatus(long accountId, EntityStatusEnum status)
        {
            AccountModel account = _dbSet.SingleOrDefault(a => a.Id == accountId);
            if (account == null)
            {
                throw new ObjectNotFoundException
                    (string.Format("Did not find an account with Account ID {0}", accountId));
            }
            else
            {
                if (_securityUtility.IsCurrentUser(account.IdentityUserId) ||
                    AuthenticationService.HasPermission(AccountPermission.UpdateNotOwned))
                {
                    // TODO: Remove personal info when deleting an account
                    account.Status = status;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new UnauthorizedAccessException("Authorization failed when attempting to set account status.");
                }
            }
        }

        public async Task AddAsync(AccountModel account)
        {
            Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task SetIsPublic(string userId, bool IsPublic)
        {
            AccountModel account = await _context.Accounts.SingleOrDefaultAsync(a=>a.IdentityUserId == userId);
            if (account == null)
            {
                throw new ObjectNotFoundException
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
                }
                else
                {
                    throw new UnauthorizedAccessException("Authorization failed when attempting to set IsPublic property.");
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

                    _context.SaveChanges();
                    transaction.Commit();
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
            string currentUserId = _securityUtility.GetUserId();

            if (currentUserId != account.IdentityUserId && !AuthenticationService.HasPermission(AccountPermission.UpdatePhotoNotOwned))
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
            string currentUserId = _securityUtility.GetUserId();
            string uploadPath = _fileService.Upload(file, "accountPhotos/");

            PhotoModel photo = new PhotoModel
            {
                Url = uploadPath,
                UploadedById = currentUserId,
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
            video.AddedBy = _securityUtility.GetUserId();
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