using Dribbly.Model;
using Dribbly.Model.Teams;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
{
    public class TeamsRepository : BaseRepository<TeamModel>, ITeamsRepository
    {
        private readonly IAuthContext _context;

        public TeamsRepository(IAuthContext context) : base(context.Teams)
        {
            _context = context;
        }

        public IEnumerable<TeamModel> GetAll()
        {
            return _dbSet;
        }

        public async Task<bool> IsTeamManagerAsync(long teamId, long accountId)
        {
            return await _context.Teams.AnyAsync(t => t.Id == teamId && t.ManagedById == accountId);
        }
    }
    public interface ITeamsRepository : IBaseRepository<TeamModel>
    {
        IEnumerable<TeamModel> GetAll();
        Task<bool> IsTeamManagerAsync(long teamId, long accountId);
    }
}