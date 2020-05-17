using DribblyAuthAPI.Models.Courts;
using DribblyAuthAPI.Models.Games;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DribblyAuthAPI.Services
{
    public interface ICourtsService
    {
        IEnumerable<CourtModel> GetAll();

        CourtModel GetCourt(long id);

        IEnumerable<GameModel> GetCourtGames(long courtId);

        IEnumerable<PhotoModel> GetCourtPhotos(long courtId);

        long Register(CourtModel court);

        IEnumerable<PhotoModel> AddPhotos(long courtId);

        void UpdateCourt(CourtModel court);

        void UpdateCourtPhoto(long courtId);

        Task DeletePhotoAsync(long courtId, long photoId);

        Task<IEnumerable<CourtModel>> FindCourtsAsync(CourtSearchInputModel input);
    }
}