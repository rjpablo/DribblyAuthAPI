using Dribbly.Model;
using Dribbly.Model.Entities;
using Dribbly.Model.Enums;
using Dribbly.Model.Games;
using System.Collections.Generic;
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

        public async Task<List<GamePlayerModel>> UpdateGamePlayerStats(List<long> accountIds, GameModel game)
        {
            List<GamePlayerModel> result = new List<GamePlayerModel>();
            var gamePlayers = await _context.GamePlayers.Include(g => g.TeamMembership.Account.User)
                .Where(g => g.GameId == game.Id && accountIds.Contains(g.TeamMembership.MemberAccountId))
                .ToListAsync();
            var shotsMade = await _context.Shots
                    .Where(s => accountIds.Contains(s.PerformedById.Value) && s.GameId == game.Id && !s.IsMiss)
                    .ToListAsync();
            var fouls = await _context.MemberFouls
                .Where(f => accountIds.Contains(f.PerformedById.Value) && f.GameId == game.Id)
                .ToListAsync();
            var otherPlays = await _context.GameEvents.Where(e => accountIds.Contains(e.PerformedById.Value) && e.GameId == game.Id &&
            e.Type != GameEventTypeEnum.ShotMade && e.Type != GameEventTypeEnum.ShotMissed &&
            e.Type != GameEventTypeEnum.FoulCommitted).ToListAsync();

            foreach (var gamePlayer in gamePlayers)
            {
                var accountId = gamePlayer.TeamMembership.MemberAccountId;
                gamePlayer.Points = shotsMade
                    .Where(s => s.PerformedById == accountId && s.GameId == game.Id && !s.IsMiss)
                    .Sum(s => s.Points);
                gamePlayer.Fouls = fouls.Count(f => f.PerformedById == accountId);
                gamePlayer.HasFouledOut = gamePlayer.Fouls > game.PersonalFoulLimit;
                result.Add(gamePlayer);
                gamePlayer.Blocks = otherPlays.Count(p => p.Type == GameEventTypeEnum.ShotBlock && p.PerformedById == accountId);
                gamePlayer.Assists = otherPlays.Count(p => p.Type == GameEventTypeEnum.Assist && p.PerformedById == accountId);
                gamePlayer.Rebounds = otherPlays.Count(p => p.PerformedById == accountId &&
                 (p.Type == GameEventTypeEnum.DefensiveRebound || p.Type == GameEventTypeEnum.OffensiveRebound));
            }
            await _context.SaveChangesAsync();
            return result;
        }

    }

    public interface IGamePlayersRepository : IBaseRepository<GamePlayerModel>
    {
        Task<GamePlayerModel> UpdateGamePlayerStats(long accountId, long gameId);
        Task<List<GamePlayerModel>> UpdateGamePlayerStats(List<long> accountIds, GameModel game);
    }
}