using Dribbly.Model;
using Dribbly.Model.Entities;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
{
    public class GamePlayersRepository : BaseRepository<GamePlayerModel>, IGamePlayersRepository
    {
        private IAuthContext _context;
        public GamePlayersRepository(IAuthContext context) : base(context.GamePlayers)
        {
            _context = context;
        }

        public async Task<GamePlayerModel> UpdateGamePlayerStats(long accountId, long gameId)
        {
            GamePlayerModel gamePlayer = await Get(g => g.Id == gameId)
                .FirstOrDefaultAsync();

            _context.GamePlayers.SingleOrDefault(g => g.TeamMembership.Account.Id == accountId
                                                && g.GameId == gameId);
            var totalPoints = await _context.Shots
                .Where(s => s.PerformedById == accountId && s.GameId == gameId && !s.IsMiss)
                .SumAsync(s => s.Points);
            gamePlayer.Points = totalPoints;
            await _context.SaveChangesAsync();
            return gamePlayer;
        }

    }

    public interface IGamePlayersRepository : IBaseRepository<GamePlayerModel>
    {
        Task<GamePlayerModel> UpdateGamePlayerStats(long accountId, long gameId);
    }
}