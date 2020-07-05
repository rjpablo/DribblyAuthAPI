using Dribbly.Model.Account;
using Dribbly.Model.Courts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public interface IAccountsService
    {
        Task<AccountModel> GetAccountByUsername(string userName);

        Task AddAsync(AccountModel account);

        Task<AccountSettingsModel> GetAccountSettingsAsync(string userId);

        Task<PhotoModel> UploadPrimaryPhotoAsync(long accountId);

        Task<IEnumerable<PhotoModel>> GetAccountPhotosAsync(int accountId);
    }
}