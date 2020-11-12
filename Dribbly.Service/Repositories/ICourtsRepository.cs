using Dribbly.Model.Courts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
{
    public interface ICourtsRepository
    {
        IEnumerable<CourtModel> GetAll();

        Task<IEnumerable<CourtModel>> FindCourtsAsync(CourtSearchInputModel input);

        Task<long> GetOwnerId(long courtId);
    }
}