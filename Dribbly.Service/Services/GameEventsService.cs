using Dribbly.Core.Exceptions;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Entities;
using Dribbly.Model.Enums;
using Dribbly.Model.Fouls;
using Dribbly.Model.GameEvents;
using Dribbly.Service.Repositories;
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
        }

        public async Task<UpsertFoulResultModel> UpsertFoulAsync(MemberFoulModel foul)
        {
            return await _memberFoulsRepository.UpsertFoul(foul);
        }

        public async Task<GameEventModel> UpsertAsync(GameEventModel gameEvent)
        {
            _gameEventsRepository.Upsert(gameEvent);
            await _context.SaveChangesAsync();
            return gameEvent;
        }

        public async Task<UpdateGameEventResultModel> UpdateAsync(UpdateGameEventInputModel input)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                UpdateGameEventResultModel result = new UpdateGameEventResultModel();
                try
                {
                    if (input.Type == GameEventTypeEnum.ShotMade || input.Type == GameEventTypeEnum.ShotMissed)
                    {
                        var shot = await _context.Shots.SingleOrDefaultAsync(s => s.Id == input.Id);
                        long origPlayerAccountId = shot.PerformedById.Value;
                        bool playerChanged = input.PerformedById != origPlayerAccountId;
                        bool teamChanged = input.TeamId != shot.TeamId;

                        shot.PerformedById = input.PerformedById;
                        shot.TeamId = input.TeamId;
                        shot.Points = input.Points;
                        shot.IsMiss = input.IsMiss;
                        shot.Type = input.IsMiss ? GameEventTypeEnum.ShotMissed : GameEventTypeEnum.ShotMade;
                        shot.TakenById = input.PerformedById;
                        shot.Period = input.Period;
                        shot.ClockTime = input.ClockTime;
                        shot.AdditionalData = JsonConvert.SerializeObject(new { points = input.Points });
                        await _context.SaveChangesAsync();

                        result.Event = _context.GameEvents.Include(g => g.PerformedBy.ProfilePhoto)
                            .Include(g => g.PerformedBy.User).Single(e => e.Id == input.Id);
                        result.Game = await _gamesRepository.UpdateGameStats(input.GameId);
                        result.Teams = new List<GameTeamModel> { result.Game.Team1, result.Game.Team2 };
                        var gamePlayer = await _gamePlayersRepository.UpdateGamePlayerStats(input.PerformedById.Value, input.GameId);
                        result.Players.Add(gamePlayer);
                        if (playerChanged)
                        {
                            var OgGamePlayer = await (playerChanged ?
                                _gamePlayersRepository.UpdateGamePlayerStats(origPlayerAccountId, input.GameId) :
                                Task.FromResult<GamePlayerModel>(null));
                            result.Players.Add(OgGamePlayer);
                        }

                        await _context.SaveChangesAsync();
                        tx.Commit();
                    }

                    return result;
                }
                catch (Exception)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }
    }

    public interface IGameEventsService
    {
        Task<UpsertFoulResultModel> UpsertFoulAsync(MemberFoulModel foul);
        Task<GameEventModel> UpsertAsync(GameEventModel gameEvent);
        Task<UpdateGameEventResultModel> UpdateAsync(UpdateGameEventInputModel input);
    }
}