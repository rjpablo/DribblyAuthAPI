using Dribbly.Model;
using Dribbly.Model.Seasons;
using System.Collections.Generic;

namespace Dribbly.Service.Repositories
{
    public interface ISeasonsRepository: IBaseRepository<SeasonModel>
    {
        IEnumerable<SeasonModel> GetAll();
    }
    public class SeasonsRepository : BaseRepository<SeasonModel>, ISeasonsRepository
    {
        public SeasonsRepository(IAuthContext context) : base(context.Seasons) { }

        public IEnumerable<SeasonModel> GetAll()
        {
            return _dbSet;
        }
    }
}