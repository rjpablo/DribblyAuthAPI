using Dribbly.Model.Courts;
using Dribbly.Model.Games;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public interface ICourtsService
    {
        Task<IEnumerable<CourtModel>> GetAllAsync();

        Task<CourtModel> GetCourtAsync(long id);

        IEnumerable<GameModel> GetCourtGames(long courtId);

        IEnumerable<PhotoModel> GetCourtPhotos(long courtId);

        long Register(CourtModel court);

        IEnumerable<PhotoModel> AddPhotos(long courtId);

        void UpdateCourt(CourtModel court);

        Task UpdateCourtPhoto(long courtId);

        Task DeletePhotoAsync(long courtId, long photoId);

        Task<IEnumerable<CourtModel>> FindCourtsAsync(CourtSearchInputModel input);
    }
}