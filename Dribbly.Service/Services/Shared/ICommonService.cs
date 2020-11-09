using Dribbly.Model.Courts;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Dribbly.Service.Services.Shared
{
    public interface ICommonService
    {
        #region User Activities
        Task AddUserPostActivity(UserActivityTypeEnum activityType, long postId);
        Task AddUserAccountActivity(UserActivityTypeEnum activityType, long accountId);
        Task AddAccountPhotoActivitiesAsync(UserActivityTypeEnum activityType, long accountId, params PhotoModel[] photos);
        Task AddAccountVideoActivitiesAsync(UserActivityTypeEnum activityType, long accountId, params VideoModel[] photos);
        #endregion

        string GetUserId();
        string TryGetRequestData(HttpRequest request);
    }
}