using Dribbly.Model;
using Dribbly.Model.Teams;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
{
    public interface ITeamsRepository
    {
        IEnumerable<TeamModel> GetAll();
    }
    public class TeamsRepository : BaseRepository<TeamModel>, ITeamsRepository
    {
        public TeamsRepository(IAuthContext context) : base(context.Teams) { }

        public IEnumerable<TeamModel> GetAll()
        {
            return _dbSet;
        }
    }
}