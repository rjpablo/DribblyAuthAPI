using Dribbly.Core.Exceptions;
using Dribbly.Core.Models;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Entities;
using Dribbly.Model.Enums;
using Dribbly.Model.Fouls;
using Dribbly.Model.GameEvents;
using Dribbly.Model.Games;
using Dribbly.Model.Play;
using Dribbly.Service.Hubs;
using Dribbly.Service.Repositories;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public class GameEventsService : BaseEntityService<GameEventModel>, IGameEventsService
    {
        IAuthContext _context;
        ISecurityUtility _securityUtility;
        private readonly IIndexedEntitysRepository _indexedEntitysRepository;
        private readonly IMemberFoulsRepository _memberFoulsRepository;
        private readonly IBaseRepository<GameEventModel> _gameEventsRepository;
        private readonly IGamesRepository _gamesRepository;
        private readonly IGamePlayersRepository _gamePlayersRepository;
        private readonly IBaseRepository<GameEventModel> _gameEventsRepo;
        private readonly IShotsRepository _shotsRepository;
        private static IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<GameHub>();

        public GameEventsService(IAuthContext context,
            ISecurityUtility securityUtility,
            IIndexedEntitysRepository indexedEntitysRepository) : base(context.GameEvents, context)
        {
            _context = context;
            _securityUtility = securityUtility;
            _indexedEntitysRepository = indexedEntitysRepository;
            _memberFoulsRepository = new MemberFoulsRepository(context);
            _gameEventsRepository = new BaseRepository<GameEventModel>(context.GameEvents);
            _gamesRepository = new GamesRepository(context);
            _gamePlayersRepository = new GamePlayersRepository(context);
            _gameEventsRepo = new BaseRepository<GameEventModel>(context.GameEvents);
            _shotsRepository = new ShotsRepository(context);
        }

        public async Task<UpsertFoulResultModel> UpsertFoulAsync(MemberFoulModel foul)
        {
            return await _memberFoulsRepository.UpsertFoul(foul);
        }

        public async Task<GameEventModel> UpsertAsync(GameEventModel gameEvent)
        {
            _gameEventsRepository.Upsert(gameEvent);

            if (gameEvent.Type == GameEventTypeEnum.Steal)
            {
                var performedBy = _context.GamePlayers.SingleOrDefault(g => g.TeamMembership.Account.Id == gameEvent.PerformedById
                                        && g.GameId == gameEvent.GameId && g.GameTeam.TeamId == gameEvent.TeamId);
                var gameTeam = await _context.GameTeams
                                .SingleOrDefaultAsync(t => t.TeamId == gameEvent.TeamId && t.GameId == gameEvent.GameId);
                gameTeam.Steals++;
                performedBy.Steals++;
            }

            await _context.SaveChangesAsync();
            return gameEvent;
        }

        public async Task<UpdateGameEventResultModel> UpdateAsync(UpdateGameEventInputModel input)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                UpdateGameEventResultModel result = new UpdateGameEventResultModel();
                List<long> accountIdsToUpdate = new List<long>();
                try
                {
                    long origPlayerAccountId;
                    bool playerChanged = false;

                    if (input.Type == GameEventTypeEnum.ShotMade || input.Type == GameEventTypeEnum.ShotMissed)
                    {
                        var shot = await _context.Shots.SingleOrDefaultAsync(s => s.Id == input.Id);
                        origPlayerAccountId = shot.PerformedById.Value;
                        playerChanged = input.PerformedById != origPlayerAccountId;
                        bool teamChanged = input.TeamId != shot.TeamId;

                        shot.PerformedById = input.PerformedById;
                        shot.TeamId = input.TeamId;
                        shot.Points = input.Points;
                        shot.IsMiss = input.IsMiss;
                        shot.Type = input.IsMiss ? GameEventTypeEnum.ShotMissed : GameEventTypeEnum.ShotMade;
                        shot.Period = input.Period;
                        shot.ClockTime = input.ClockTime;
                        shot.AdditionalData = JsonConvert.SerializeObject(new { points = input.Points });
                        await _context.SaveChangesAsync();

                    }
                    if (input.Type == GameEventTypeEnum.FoulCommitted)
                    {
                        var foul = await _context.MemberFouls.SingleOrDefaultAsync(f => f.Id == input.Id);
                        origPlayerAccountId = foul.PerformedById.Value;
                        playerChanged = input.PerformedById != origPlayerAccountId;
                        bool teamChanged = input.TeamId != foul.TeamId;
                        foul.PerformedById = input.PerformedById;
                        foul.TeamId = input.TeamId;
                        foul.Type = input.Type;
                        foul.Period = input.Period;
                        foul.ClockTime = input.ClockTime;
                        foul.FoulId = input.FoulId.Value;
                        var foulType = Constants.Fouls.Single(f => f.Id == input.FoulId.Value);
                        foul.AdditionalData = JsonConvert.SerializeObject(new { foulName = foulType.Name, foulId = foul.Id });
                        await _context.SaveChangesAsync();
                    }
                    else if (input.Type == GameEventTypeEnum.FreeThrowMade || input.Type == GameEventTypeEnum.FreeThrowMissed)
                    {
                        var shot = await _context.Shots.SingleOrDefaultAsync(s => s.Id == input.Id);
                        origPlayerAccountId = shot.PerformedById.Value;
                        playerChanged = input.PerformedById != origPlayerAccountId;
                        bool teamChanged = input.TeamId != shot.TeamId;

                        shot.PerformedById = input.PerformedById;
                        shot.TeamId = input.TeamId;
                        shot.IsMiss = input.IsMiss;
                        shot.Type = input.IsMiss ? GameEventTypeEnum.FreeThrowMissed : GameEventTypeEnum.FreeThrowMade;
                        shot.Period = input.Period;
                        shot.ClockTime = input.ClockTime;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        var evt = _context.GameEvents.Single(e => e.Id == input.Id);
                        origPlayerAccountId = evt.PerformedById.Value;
                        playerChanged = input.PerformedById != origPlayerAccountId;
                        bool teamChanged = input.TeamId != evt.TeamId;

                        evt.PerformedById = input.PerformedById;
                        evt.TeamId = input.TeamId;
                        evt.Type = input.Type;
                        evt.Period = input.Period;
                        evt.ClockTime = input.ClockTime;
                        await _context.SaveChangesAsync();
                    }

                    result.Event = _context.GameEvents.Include(g => g.PerformedBy.ProfilePhoto)
                            .Include(g => g.PerformedBy.User).Single(e => e.Id == input.Id);
                    result.Game = await _gamesRepository.UpdateGameStats(input.GameId, input.Type);
                    result.Teams = new List<GameTeamModel> { result.Game.Team1, result.Game.Team2 };
                    accountIdsToUpdate.Add(input.PerformedById.Value);
                    if (playerChanged)
                    {
                        accountIdsToUpdate.Add(origPlayerAccountId);
                    }

                    result.Players = await _gamePlayersRepository.UpdateGamePlayerStats(accountIdsToUpdate.Distinct().ToList(), result.Game);
                    await _context.SaveChangesAsync();
                    tx.Commit();
                    BroadcastUpsertGameEvent(result.Event);
                    return result;
                }
                catch (Exception)
                {
                    BroadcastDeleteGameEvent(result.Event);
                    tx.Rollback();
                    throw;
                }
            }
        }

        public async Task<UpdateGameEventResultModel> UpsertFreeThrowAsync(UpsertFreeThrowInputModel input)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    UpdateGameEventResultModel result = new UpdateGameEventResultModel();
                    List<long> accountIdsToUpdate = new List<long>();
                    accountIdsToUpdate.Add(input.PerformedById.Value);
                    ShotModel shot = null;
                    List<ShotModel> shots = new List<ShotModel>();
                    GameModel game = await _gamesRepository.Get(g => g.Id == input.GameId,
                        $"{nameof(GameModel.Team1)},{nameof(GameModel.Team2)}")
                        .FirstOrDefaultAsync();
                    if (game == null)
                        throw new DribblyObjectNotFoundException($"A game with ID {input.GameId} does not exist.");

                    var shootersTeam = game.Team1.TeamId == input.TeamId ? game.Team1 : game.Team2;
                    var pointsMade = input.AttemptResults.Count(a => a);

                    for (int i = 0; i < input.AttemptResults.Count; i++)
                    {
                        var isMade = input.AttemptResults.ElementAt(i);
                        shot = new ShotModel
                        {
                            Points = 1,
                            IsMiss = !isMade,
                            GameId = input.GameId,
                            TeamId = input.TeamId,
                            PerformedById = input.PerformedById,
                            Type = isMade ? GameEventTypeEnum.FreeThrowMade : GameEventTypeEnum.FreeThrowMissed,
                            Period = input.Period,
                            ClockTime = input.ClockTime,
                            ShotType = ShotTypeEnum.FreeThrow
                        };
                        _context.SetEntityState(shot.Game, EntityState.Unchanged);
                        shot.DateAdded = DateTime.UtcNow;
                        _shotsRepository.Add(shot);
                        shots.Add(shot);
                    }
                    await _context.SaveChangesAsync();

                    var gamePlayer = _context.GamePlayers.SingleOrDefault(g => g.TeamMembership.Account.Id == input.PerformedById
                                            && g.GameId == input.GameId && g.GameTeam.TeamId == input.TeamId);
                    // TODO: add gamePlayer null check

                    gamePlayer.FTA += input.AttemptResults.Count;
                    shootersTeam.FTA += input.AttemptResults.Count;

                    if (pointsMade > 0)
                    {
                        shootersTeam.Points += pointsMade;
                        if (shootersTeam.TeamId == game.Team1.TeamId)
                        {
                            game.Team1Score = shootersTeam.Points;
                        }
                        else if (shot.TeamId == game.Team2.TeamId)
                        {
                            game.Team2Score = shootersTeam.Points;
                        }

                        gamePlayer.Points += pointsMade;
                        gamePlayer.FTM += pointsMade;
                        shootersTeam.FTM += pointsMade;
                        await _context.SaveChangesAsync();
                    }

                    result.Players.Add(gamePlayer);
                    result.Teams.Add(shootersTeam);
                    result.Game = game;

                    if (input.Rebound != null)
                    {
                        accountIdsToUpdate.Add(input.Rebound.PerformedById.Value);
                        input.Rebound.ShotId = shot.Id;
                        _gameEventsRepo.Upsert(input.Rebound);
                        await _context.SaveChangesAsync();
                        var reboundedBy = _context.GamePlayers.SingleOrDefault(g => g.TeamMembership.Account.Id == input.Rebound.PerformedById
                                                && g.GameId == input.Rebound.GameId && g.GameTeam.TeamId == input.Rebound.TeamId);
                        var rebounds = await _context.GameEvents
                            .Where(e => (e.Type == GameEventTypeEnum.OffensiveRebound || e.Type == GameEventTypeEnum.DefensiveRebound)
                            && e.PerformedById == input.Rebound.PerformedById && e.GameId == input.Rebound.GameId).ToListAsync();
                        reboundedBy.Rebounds = rebounds.Count();
                        reboundedBy.OReb = rebounds.Count(e => e.Type == GameEventTypeEnum.OffensiveRebound);
                        reboundedBy.DReb = rebounds.Count(e => e.Type == GameEventTypeEnum.DefensiveRebound);
                        var reboundersTeam = game.Team1.TeamId == input.Rebound.TeamId ? game.Team1 : game.Team2;
                        reboundersTeam.Rebounds++;
                        await _context.SaveChangesAsync();
                        if (reboundersTeam != shootersTeam)
                        {
                            result.Teams.Add(reboundersTeam);
                        }
                        result.Players.Add(reboundedBy);
                    }

                    transaction.Commit();

                    foreach(var s in shots)
                    {
                        BroadcastUpsertGameEvent(s);
                    }
                    BroadcastUpsertGameEvent(input.Rebound);

                    return result;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public async Task<UpsertShotResultModel> RecordShotAsync(ShotDetailsInputModel input)
        {
            GameModel game = await _gamesRepository.Get(g => g.Id == input.Shot.GameId,
                $"{nameof(GameModel.Team1)},{nameof(GameModel.Team2)}")
                .FirstOrDefaultAsync();
            var shootersTeam = game.Team1.TeamId == input.Shot.TeamId ? game.Team1 : game.Team2;
            var opponentTeam = game.Team1.TeamId != shootersTeam.TeamId ? game.Team1 : game.Team2;

            if (game == null)
            {
                throw new DribblyObjectNotFoundException($"A game with ID {input.Shot.Game.Id} does not exist.");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.SetEntityState(input.Shot.Game, EntityState.Unchanged);
                    input.Shot.Type = input.Shot.IsMiss ?
                        Model.Enums.GameEventTypeEnum.ShotMissed :
                        Model.Enums.GameEventTypeEnum.ShotMade;
                    input.Shot.AdditionalData = JsonConvert.SerializeObject(new { points = input.Shot.Points });
                    input.Shot.DateAdded = DateTime.UtcNow;
                    _shotsRepository.Add(input.Shot);
                    await _context.SaveChangesAsync();

                    var gamePlayer = _context.GamePlayers.SingleOrDefault(g => g.TeamMembership.Account.Id == input.Shot.PerformedById
                                            && g.GameId == input.Shot.GameId && g.GameTeam.TeamId == input.Shot.TeamId);
                    gamePlayer.FGA++;
                    shootersTeam.FGA++;
                    if (input.Shot.Points == 3)
                    {
                        gamePlayer.ThreePA++;
                        shootersTeam.ThreePA++;
                    }
                    await _context.SaveChangesAsync();

                    var result = new UpsertShotResultModel();
                    if (!input.Shot.IsMiss)
                    {
                        shootersTeam.Points = await _context.Shots
                            .Where(s => s.TeamId == shootersTeam.TeamId && s.GameId == input.Shot.GameId && !s.IsMiss)
                            .SumAsync(s => s.Points);
                        if (shootersTeam.TeamId == game.Team1.TeamId)
                        {
                            game.Team1Score = shootersTeam.Points;
                        }
                        else if (input.Shot.TeamId == game.Team2.TeamId)
                        {
                            game.Team2Score = shootersTeam.Points;
                        }

                        // TODO: add gamePlayer null check

                        gamePlayer.Points += input.Shot.Points;
                        gamePlayer.FGM++;
                        shootersTeam.FGM++;
                        if (input.Shot.Points == 3)
                        {
                            gamePlayer.ThreePM++;
                            shootersTeam.ThreePM++;
                        }
                        await _context.SaveChangesAsync();
                    }

                    result.TakenBy = gamePlayer;
                    result.Team1 = game.Team1;
                    result.Team1Score = game.Team1Score;
                    result.Team2 = game.Team2;
                    result.Team2Score = game.Team2Score;

                    if (input.WithFoul)
                    {
                        input.Foul.Game = game;
                        input.Foul.ShotId = input.Shot.Id;
                        result.FoulResult = await _memberFoulsRepository.UpsertFoul(input.Foul);
                    }

                    if (input.WithBlock)
                    {
                        input.Block.ShotId = input.Shot.Id;
                        _context.SetEntityState(input.Block.PerformedBy, EntityState.Unchanged);
                        _gameEventsRepo.Upsert(input.Block);
                        await _context.SaveChangesAsync();
                        var blocker = _context.GamePlayers.SingleOrDefault(g => g.TeamMembership.Account.Id == input.Block.PerformedById
                                                && g.GameId == input.Shot.GameId && g.GameTeam.TeamId == input.Block.TeamId);
                        blocker.Blocks = await _context.GameEvents.CountAsync(e => e.Type == GameEventTypeEnum.ShotBlock
                        && e.PerformedById == input.Block.PerformedById && e.GameId == input.Block.GameId);
                        result.BlockResult = new BlockResultModel
                        {
                            TotalBlocks = blocker.Blocks
                        };
                        opponentTeam.Blocks++;
                        await _context.SaveChangesAsync();
                    }

                    if (input.WithAssist)
                    {
                        input.Assist.ShotId = input.Shot.Id;
                        _context.SetEntityState(input.Assist.PerformedBy, EntityState.Unchanged);
                        _gameEventsRepo.Upsert(input.Assist);
                        await _context.SaveChangesAsync();
                        var assistedBy = _context.GamePlayers.SingleOrDefault(g => g.TeamMembership.Account.Id == input.Assist.PerformedById
                                                && g.GameId == input.Shot.GameId && g.GameTeam.TeamId == input.Assist.TeamId);
                        assistedBy.Assists = await _context.GameEvents.CountAsync(e => e.Type == GameEventTypeEnum.Assist
                        && e.PerformedById == input.Assist.PerformedById && e.GameId == input.Assist.GameId);
                        result.AssistResult = new AssistResultModel
                        {
                            TotalAssists = assistedBy.Assists
                        };
                        shootersTeam.Assists++;
                        await _context.SaveChangesAsync();
                    }

                    if (input.WithRebound)
                    {
                        input.Rebound.ShotId = input.Shot.Id;
                        _context.SetEntityState(input.Rebound.PerformedBy, EntityState.Unchanged);
                        _gameEventsRepo.Upsert(input.Rebound);
                        await _context.SaveChangesAsync();
                        var reboundedBy = _context.GamePlayers.SingleOrDefault(g => g.TeamMembership.Account.Id == input.Rebound.PerformedById
                                                && g.GameId == input.Rebound.GameId && g.GameTeam.TeamId == input.Rebound.TeamId);
                        var rebounds = await _context.GameEvents
                            .Where(e => (e.Type == GameEventTypeEnum.OffensiveRebound || e.Type == GameEventTypeEnum.DefensiveRebound)
                            && e.PerformedById == input.Rebound.PerformedById && e.GameId == input.Rebound.GameId).ToListAsync();
                        reboundedBy.Rebounds = rebounds.Count();
                        reboundedBy.OReb = rebounds.Count(e => e.Type == GameEventTypeEnum.OffensiveRebound);
                        reboundedBy.DReb = rebounds.Count(e => e.Type == GameEventTypeEnum.DefensiveRebound);
                        result.ReboundResult = new ReboundResultModel
                        {
                            PerformedById = reboundedBy.AccountId,
                            TotalRebounds = reboundedBy.Rebounds
                        };
                        var reboundersTeam = game.Team1.TeamId == input.Rebound.TeamId ? game.Team1 : game.Team2;
                        reboundersTeam.Rebounds++;
                        await _context.SaveChangesAsync();
                    }

                    transaction.Commit();

                    BroadcastUpsertGameEvent(input.Shot);
                    BroadcastUpsertGameEvent(input.Foul);
                    BroadcastUpsertGameEvent(input.Block);
                    BroadcastUpsertGameEvent(input.Assist);
                    BroadcastUpsertGameEvent(input.Rebound);

                    return result;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

        }

        public async Task<UpdateGameEventResultModel> DeleteAsync(long gameEventId)
        {
            //TODO: Only the game manager should be allowed to perform this action
            using (var tx = _context.Database.BeginTransaction())
            {
                UpdateGameEventResultModel result = new UpdateGameEventResultModel();
                result.IsDelete = true;
                List<long> playersToUpdate = new List<long>();

                var events = _context.GameEvents.Where(e => e.Id == gameEventId || e.ShotId == gameEventId).ToList();
                var gameId = events.First().GameId;
                try
                {
                    // DELETE non-Field Goals first because they reference the Field Goal
                    foreach (var evt in events.Where(e => e.Type != GameEventTypeEnum.ShotMade &&
                    e.Type != GameEventTypeEnum.ShotMissed))
                    {
                        if (evt.PerformedById != null)
                        {
                            playersToUpdate.Add(evt.PerformedById.Value);
                        }

                        if (evt.Type == GameEventTypeEnum.FoulCommitted)
                        {
                            _context.MemberFouls.Remove((MemberFoulModel)evt);
                        }
                        else
                        {
                            _context.GameEvents.Remove(evt);
                        }

                        await _context.SaveChangesAsync();
                    }

                    // Delete field goal
                    var shot = events.SingleOrDefault(e => e.Type == GameEventTypeEnum.ShotMade ||
                    e.Type == GameEventTypeEnum.ShotMissed);
                    if (shot != null)
                    {
                        playersToUpdate.Add(shot.PerformedById.Value);

                        _context.GameEvents.Remove(shot);
                        _context.Shots.Remove((Model.Games.ShotModel)shot);
                        await _context.SaveChangesAsync();
                    }

                    result.Game = await _gamesRepository.UpdateGameStats(gameId);
                    result.Teams.Add(result.Game.Team1);
                    result.Teams.Add(result.Game.Team2);
                    result.Players = await _gamePlayersRepository.UpdateGamePlayerStats(playersToUpdate.Distinct().ToList(), result.Game);

                    tx.Commit();

                    events.ForEach(evt => BroadcastDeleteGameEvent(evt));

                    return result;
                }
                catch (Exception)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public async Task RecordTurnoverAsync(GameEventModel turnover)
        {
            _gameEventsRepository.Upsert(turnover);
            var performedBy = _context.GamePlayers.SingleOrDefault(g => g.TeamMembership.Account.Id == turnover.PerformedById
                                    && g.GameId == turnover.GameId && g.GameTeam.TeamId == turnover.TeamId);
            performedBy.Turnovers++;
            var gameTeam = await _context.GameTeams
                            .SingleOrDefaultAsync(t => t.TeamId == turnover.TeamId && t.GameId == turnover.GameId);
            gameTeam.Turnovers++;
            await _context.SaveChangesAsync();
            BroadcastUpsertGameEvent(turnover);
        }

        private void BroadcastUpsertGameEvent(GameEventModel gameEvent)
        {
            if (gameEvent != null)
                _hubContext.Clients.Group(gameEvent.GameId.ToString()).upsertGameEvent(gameEvent);
        }

        private void BroadcastDeleteGameEvent(GameEventModel gameEvent)
        {
            if (gameEvent != null)
                _hubContext.Clients.Group(gameEvent.GameId.ToString()).deleteGameEvent(gameEvent);
        }
    }

    public interface IGameEventsService
    {
        Task<UpdateGameEventResultModel> DeleteAsync(long gameEventId);
        Task<UpsertFoulResultModel> UpsertFoulAsync(MemberFoulModel foul);
        Task<GameEventModel> UpsertAsync(GameEventModel gameEvent);
        Task<UpdateGameEventResultModel> UpdateAsync(UpdateGameEventInputModel input);
        Task RecordTurnoverAsync(GameEventModel turnover);
        Task<UpsertShotResultModel> RecordShotAsync(ShotDetailsInputModel input);
        Task<UpdateGameEventResultModel> UpsertFreeThrowAsync(UpsertFreeThrowInputModel input);
    }
}