using DribblyAuthAPI.Models.Courts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DribblyAuthAPI.Repositories
{
    public interface ICourtsRepository
    {
        IEnumerable<CourtModel> GetAll();

        Task<IEnumerable<CourtModel>> FindCourtsAsync(CourtSearchInputModel input);
    }
}