using Dribbly.Model.Courts;
using Dribbly.Model.Bookings;
using Dribbly.Model.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Dribbly.Model.Games;

namespace Dribbly.Service.Services
{
    public interface ICourtsService
    {
        Task<IEnumerable<CourtDetailsViewModel>> GetAllActiveAsync();

        Task<CourtDetailsViewModel> GetCourtAsync(long id);

        IEnumerable<BookingModel> GetCourtBookings(long courtId);

        Task<IEnumerable<GameModel>> GetCourtGamesAsync(long courtId);

        IEnumerable<PhotoModel> GetCourtPhotos(long courtId);

        Task<IEnumerable<VideoModel>> GetCourtVideosAsync(long courtId);

        Task<CourtReviewModalModel> GetCodeReviewModalAsync(long courtId);

        Task DeleteCourtAsync(long courtId);

        Task<long> RegisterAsync(CourtModel court);

        Task<IEnumerable<PhotoModel>> AddPhotosAsync(long courtId);

        Task UpdateCourtAsync(CourtModel court);

        Task UpdateCourtPropertiesAsync(GenericEntityUpdateInputModel input);

        Task UpdateCourtPhoto(long courtId);

        Task DeletePhotoAsync(long courtId, long photoId);

        Task<IEnumerable<CourtModel>> FindActiveCourtsAsync(CourtSearchInputModel input);

        Task<VideoModel> AddVideoAsync(long courtId, VideoModel video, HttpPostedFile file);

        Task DeleteCourtVideoAsync(long courtId, long videoId);

        Task<FollowResultModel> FollowCourtAsync(long courtId, bool isFollowing);

        #region Reviews

        Task<IEnumerable<CourtReviewModel>> GetReviewsAsync(long courtId);

        Task SubmitReviewAsync(CourtReviewModel review);

        #endregion

    }
}