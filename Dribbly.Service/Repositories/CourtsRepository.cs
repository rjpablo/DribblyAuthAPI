using Dribbly.Model;
using Dribbly.Model.Courts;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
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

        public async Task<string> GetOwnerId(long courtId)
        {
            return (await _dbSet.SingleOrDefaultAsync(c => c.Id == courtId))?.OwnerId;
        }

    }
}