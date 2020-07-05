using Dribbly.Authentication.Services;
using Dribbly.Core.Enums.Permissions;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Account;
using Dribbly.Model.Accounts;
using Dribbly.Model.Courts;
using Dribbly.Service.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

        public async Task AddAsync(AccountModel account)
        {
            Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task<AccountSettingsModel> GetAccountSettingsAsync(string userId)
        {
            return await Task.FromResult(new AccountSettingsModel());
        }

        public async Task<IEnumerable<PhotoModel>> GetAccountPhotosAsync(int accountId)
        {
            return await _context.AccountPhotos.Include(p=>p.Photo)
                .Where(p => p.AccountId == accountId).Select(p => p.Photo)
                .ToListAsync();
        }

        public async Task<PhotoModel> UploadPrimaryPhotoAsync(long accountId)
        {
            AccountModel account = GetById(accountId);
            string currentUserId = _securityUtility.GetUserId();

            if(currentUserId != account.IdentityUserId && !AuthenticationService.HasPermission(AccountPermission.UpdatePhotoNotOwned))
            {
                throw new UnauthorizedAccessException("Authorization failed when attempting to update account primary photo.");
            }

            HttpFileCollection files = HttpContext.Current.Request.Files;
            string uploadPath = _fileService.Upload(files[0], "accountPhotos/");
            
            using(var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    PhotoModel photo = new PhotoModel
                    {
                        Url = uploadPath,
                        UploadedById = currentUserId,
                        DateAdded = DateTime.Now
                    };
                    _context.Photos.Add(photo);
                    await _context.SaveChangesAsync();

                    _context.AccountPhotos.Add(new AccountPhotoModel
                    {
                        PhotoId = photo.Id,
                        AccountId = account.Id
                    });
                    account.ProfilePhotoId = photo.Id;
                    Update(account);
                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    return photo;
                }
                catch(Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

    }
}