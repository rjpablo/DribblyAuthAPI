using Dribbly.Model.Courts;
using Dribbly.Model.Games;
using Dribbly.Model.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Dribbly.Service.Services
{
    public interface ICourtsService
    {
        Task<IEnumerable<CourtDetailsViewModel>> GetAllAsync();

        Task<CourtDetailsViewModel> GetCourtAsync(long id);

        IEnumerable<GameModel> GetCourtGames(long courtId);

        IEnumerable<PhotoModel> GetCourtPhotos(long courtId);

        Task<IEnumerable<VideoModel>> GetCourtVideosAsync(long courtId);

        long Register(CourtModel court);

        IEnumerable<PhotoModel> AddPhotos(long courtId);

        void UpdateCourt(CourtModel court);

        Task UpdateCourtPhoto(long courtId);

        Task DeletePhotoAsync(long courtId, long photoId);

        Task<IEnumerable<CourtModel>> FindCourtsAsync(CourtSearchInputModel input);

        Task<VideoModel> AddVideoAsync(long courtId, VideoModel video, HttpPostedFile file);

        Task DeleteCourtVideoAsync(long courtId, long videoId);

        Task<FollowResultModel> FollowCourtAsync(long courtId, bool isFollowing);

    }
}