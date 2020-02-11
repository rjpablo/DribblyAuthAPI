using DribblyAuthAPI.Models.Courts;
using System.Collections.Generic;

namespace DribblyAuthAPI.Services
{
    public interface ICourtsService
    {
        IEnumerable<CourtModel> GetAll();

        CourtModel GetCourt(long id);

        long Register(CourtModel court);

        void UpdateCourt(CourtModel court);

        void UpdateCourtPhoto(long courtId);
    }
}