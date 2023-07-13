using Dribbly.Model;
using Dribbly.Model.Games;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
{
    public class GamesRepository : BaseRepository<GameModel>, IGamesRepository
    {
        private IAuthContext _context;
        public GamesRepository(IAuthContext context) : base(context.Games) {
            _context = context;
        }

        public async Task<GameModel> UpdateGameStats(long gameId)
        {
            GameModel game = await Get(g => g.Id == gameId,
                $"{nameof(GameModel.Team1)},{nameof(GameModel.Team2)}")
                .FirstOrDefaultAsync();

                game.Team1Score = await _context.Shots
                .Where(s => s.TeamId == game.Team1.TeamId && s.GameId == gameId && !s.IsMiss)
                .SumAsync(s => s.Points);
                game.Team1.Score = game.Team1Score;

                game.Team2Score = await _context.Shots
                .Where(s => s.TeamId == game.Team2.TeamId && s.GameId == gameId && !s.IsMiss)
                .SumAsync(s => s.Points);
                game.Team2.Score = game.Team2Score;

            await _context.SaveChangesAsync();
            return game;
        }

    }

    public interface IGamesRepository : IBaseRepository<GameModel>
    {
        Task<GameModel> UpdateGameStats(long gameId);
    }
}