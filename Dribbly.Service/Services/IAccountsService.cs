using Dribbly.Model.Account;
using Dribbly.Model.Courts;
using Dribbly.Model.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Dribbly.Service.Services
{
    public interface IAccountsService
    {
        Task<AccountModel> GetAccountByUsername(string userName);

        Task AddAsync(AccountModel account);

        Task<AccountSettingsModel> GetAccountSettingsAsync(string userId);

        Task<PhotoModel> UploadPrimaryPhotoAsync(long accountId);

        Task<IEnumerable<PhotoModel>> GetAccountPhotosAsync(int accountId);

        Task DeletePhoto(int photoId, int accountId);

        Task<IEnumerable<VideoModel>> GetAccountVideosAsync(long accountId);

        Task<VideoModel> AddVideoAsync(long accountId, VideoModel video, HttpPostedFile file);
    }
}