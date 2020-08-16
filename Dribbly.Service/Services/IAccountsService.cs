using Dribbly.Model.Account;
using Dribbly.Model.Courts;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Dribbly.Service.Services
{
    public interface IAccountsService
    {
        Task<AccountModel> GetAccountByUsername(string userName);

        Task AddAsync(AccountModel account);

        Task<IEnumerable<PhotoModel>> AddAccountPhotosAsync(long accountId);

        Task<AccountSettingsModel> GetAccountSettingsAsync(string userId);

        Task<PhotoModel> UploadPrimaryPhotoAsync(long accountId);

        Task<IEnumerable<PhotoModel>> GetAccountPhotosAsync(int accountId);

        Task DeletePhoto(int photoId, int accountId);

        Task<IEnumerable<VideoModel>> GetAccountVideosAsync(long accountId);

        Task<VideoModel> AddVideoAsync(long accountId, VideoModel video, HttpPostedFile file);

        Task<IEnumerable<AccountsChoicesItemModel>> GetAccountDropDownSuggestions(AccountSearchInputModel input);

        Task SetStatus(long accountId, AccountStatusEnum status);

        Task SetIsPublic(string userId, bool IsPublic);
    }
}