using Dribbly.Model;
using Dribbly.Model.Games;
using Dribbly.Model.Teams;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
{
    public interface IShotsRepository: IBaseRepository<ShotModel>
    {
        IEnumerable<ShotModel> GetAll();
    }
    public class ShotsRepository : BaseRepository<ShotModel>, IShotsRepository
    {
        public ShotsRepository(IAuthContext context) : base(context.Shots) { }

        public IEnumerable<ShotModel> GetAll()
        {
            return _dbSet;
        }
    }
}