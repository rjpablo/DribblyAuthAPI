using DribblyAuthAPI.Models;
using DribblyAuthAPI.Models.Courts;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;

namespace DribblyAuthAPI.Repositories
{
    public class CourtsRepository : BaseRepository<CourtModel>, ICourtsRepository
    {
        public CourtsRepository(IAuthContext context) : base(context.Courts) { }

        public IEnumerable<CourtModel> GetAll()
        {
            return _dbSet;
        }

        public async Task<IEnumerable<CourtModel>> FindCourtsAsync(CourtSearchInputModel input)
        {
            return await _dbSet.Where(c => c.Name.Contains(input.Name)).ToListAsync();
        }

    }
}