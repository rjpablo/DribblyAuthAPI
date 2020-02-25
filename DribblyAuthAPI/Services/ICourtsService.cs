using DribblyAuthAPI.Models.Courts;
using DribblyAuthAPI.Models.Games;
using System.Collections.Generic;

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
    }
}