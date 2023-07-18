﻿using Dribbly.Core.Exceptions;
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
                List<long> accountIdsToUpdate = new List<long>();
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
                        shot.Period = input.Period;
                        shot.ClockTime = input.ClockTime;
                        shot.AdditionalData = JsonConvert.SerializeObject(new { points = input.Points });
                        await _context.SaveChangesAsync();

                        result.Event = _context.GameEvents.Include(g => g.PerformedBy.ProfilePhoto)
                            .Include(g => g.PerformedBy.User).Single(e => e.Id == input.Id);
                        result.Game = await _gamesRepository.UpdateGameStats(input.GameId);
                        result.Teams = new List<GameTeamModel> { result.Game.Team1, result.Game.Team2 };
                        accountIdsToUpdate.Add(input.PerformedById.Value);
                        if (playerChanged)
                        {
                            var OgGamePlayer = await (playerChanged ?
                                _gamePlayersRepository.UpdateGamePlayerStats(origPlayerAccountId, input.GameId) :
                                Task.FromResult<GamePlayerModel>(null));
                            accountIdsToUpdate.Add(origPlayerAccountId);
                        }

                        result.Players = await _gamePlayersRepository.UpdateGamePlayerStats(accountIdsToUpdate.Distinct().ToList(), result.Game);
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

        public async Task<UpdateGameEventResultModel> DeleteAsync(long gameEventId)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                UpdateGameEventResultModel result = new UpdateGameEventResultModel();
                result.IsDelete = true;
                List<long> playersToUpdate = new List<long>();

                var events = _context.GameEvents.Where(e => e.Id == gameEventId || e.ShotId == gameEventId).ToList();

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

                    result.Game = await _gamesRepository.UpdateGameStats(shot.GameId);
                    result.Teams.Add(result.Game.Team1);
                    result.Teams.Add(result.Game.Team2);
                    result.Players = await _gamePlayersRepository.UpdateGamePlayerStats(playersToUpdate.Distinct().ToList(), result.Game);

                    tx.Commit();
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
        Task<UpdateGameEventResultModel> DeleteAsync(long gameEventId);
        Task<UpsertFoulResultModel> UpsertFoulAsync(MemberFoulModel foul);
        Task<GameEventModel> UpsertAsync(GameEventModel gameEvent);
        Task<UpdateGameEventResultModel> UpdateAsync(UpdateGameEventInputModel input);
    }
}