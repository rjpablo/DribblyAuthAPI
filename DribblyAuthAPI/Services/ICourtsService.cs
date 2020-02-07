using DribblyAuthAPI.Models.Courts;
using System.Collections.Generic;

namespace DribblyAuthAPI.Services
{
    public interface ICourtsService
    {
        IEnumerable<CourtModel> GetAll();

        CourtModel GetCourt(long id);

        void Register(CourtModel court);

        void UpdateCourt(CourtModel court);
    }
}