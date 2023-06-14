using Dribbly.Model;
using Dribbly.Model.Games;
using Dribbly.Model.Leagues;
using Dribbly.Model.Teams;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
{
    public interface ILeaguesRepository: IBaseRepository<LeagueModel>
    {
        IEnumerable<LeagueModel> GetAll();
    }
    public class LeaguesRepository : BaseRepository<LeagueModel>, ILeaguesRepository
    {
        public LeaguesRepository(IAuthContext context) : base(context.Leagues) { }

        public IEnumerable<LeagueModel> GetAll()
        {
            return _dbSet;
        }
    }
}