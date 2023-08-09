using Dribbly.Core.Exceptions;
using Dribbly.Core.Extensions;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Account;
using Dribbly.Model.DTO;
using Dribbly.Model.Entities;
using Dribbly.Model.Enums;
using Dribbly.Model.GameEvents;
using Dribbly.Model.Games;
using Dribbly.Model.Notifications;
using Dribbly.Model.Play;
using Dribbly.Model.Shared;
using Dribbly.Model.Teams;
using Dribbly.Service.Enums;
using Dribbly.Service.Repositories;
using Dribbly.Service.Services.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public class GamesService : BaseEntityService<GameModel>, IGamesService
    {
        private readonly IAuthContext _context;
        private readonly ISecurityUtility _securityUtility;
        private readonly IFileService _fileService;
        private readonly IAccountRepository _accountRepo;
        private readonly INotificationsRepository _notificationsRepo;
        private readonly ICourtsRepository _courtsRepo;
        private readonly ICommonService _commonService;
        private readonly ITeamsRepository _teamsRepository;
        private readonly IBaseRepository<GameModel> _gameRepo;
        private readonly IBaseRepository<GameEventModel> _gameEventsRepo;
        private readonly IMemberFoulsRepository _memberFoulsRepository;
        private readonly IShotsRepository _shotsRepository;
        private readonly IGamePlayersRepository _gamePlayerRepo;

        public GamesService(IAuthContext context,
            ISecurityUtility securityUtility,
            IAccountRepository accountRepo,
            IFileService fileService,
            INotificationsRepository notificationsRepo,
            ICourtsRepository courtsRepo,
            ICommonService commonService,
            ITeamsRepository teamsRepository) : base(context.Games, context)
        {
            _context = context;
            _securityUtility = securityUtility;
            _accountRepo = accountRepo;
            _fileService = fileService;
            _notificationsRepo = notificationsRepo;
            _courtsRepo = courtsRepo;
            _commonService = commonService;
            _teamsRepository = teamsRepository;
            _gameRepo = new BaseRepository<GameModel>(context.Games);
            _gameEventsRepo = new BaseRepository<GameEventModel>(context.GameEvents);
            _memberFoulsRepository = new MemberFoulsRepository(context);
            _shotsRepository = new ShotsRepository(context);
            _gamePlayerRepo = new GamePlayersRepository(context);
        }

        public IEnumerable<GameModel> GetAll()
        {
            return All();
        }

        public async Task<IEnumerable<GameModel>> GetGamesAsync(GetGamesFilterModel filter)
        {
            var games = await _dbSet.Include(g => g.Court.PrimaryPhoto)
                .Include(g => g.Team1).Include(g => g.Team1.Team.Logo)
                .Include(g => g.Team2).Include(g => g.Team2.Team.Logo)
                .Where(g =>
                (!filter.TeamIds.Any() || filter.TeamIds.Contains(g.Team1.TeamId) || filter.TeamIds.Contains(g.Team2.TeamId))
                && (!filter.UpcomingOnly || g.Start > DateTime.UtcNow))
                .ToListAsync();
            return games;
        }

        public async Task<GameModel> GetGame(long id)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            //GameModel game = await _dbSet.Where(g => g.Id == id).Include(g => g.Court.PrimaryPhoto)
            //    .Include(g => g.Team1).Include(g => g.Team1.Team.Logo)
            //    .Include(g => g.Team2).Include(g => g.Team2.Team.Logo)
            //    .SingleOrDefaultAsync();

            GameModel game = await _gameRepo.Get(g => g.Id == id,
                // include team1 details
                $"{nameof(GameModel.Team1)}.{nameof(GameTeamModel.Team)}.{nameof(TeamModel.Logo)}," +
                // include team1 players details
                $"{nameof(GameModel.Team1)}.{nameof(GameTeamModel.Players)}.{nameof(GamePlayerModel.TeamMembership)}" +
                $".{nameof(TeamMembershipModel.Account)}.{nameof(AccountModel.User)}," +
                $"{nameof(GameModel.Team1)}.{nameof(GameTeamModel.Players)}.{nameof(GamePlayerModel.TeamMembership)}" +
                $".{nameof(TeamMembershipModel.Account)}.{nameof(AccountModel.ProfilePhoto)}," +
                // include team2 details
                $"{nameof(GameModel.Team2)}.{nameof(GameTeamModel.Team)}.{nameof(TeamModel.Logo)}," +
                // include team2 players details
                $"{nameof(GameModel.Team2)}.{nameof(GameTeamModel.Players)}.{nameof(GamePlayerModel.TeamMembership)}" +
                $".{nameof(TeamMembershipModel.Account)}.{nameof(AccountModel.User)}," +
                // include gameevents for play-by-play
                $"{nameof(GameModel.GameEvents)}.{nameof(GameEventModel.PerformedBy)}.{nameof(AccountModel.ProfilePhoto)}," +
                $"{nameof(GameModel.GameEvents)}.{nameof(GameEventModel.Team)}," +
                $"{nameof(GameModel.Court)}")
                .FirstOrDefaultAsync();
            if (game != null)
            {
                game.AddedBy = await _accountRepo.GetAccountBasicInfo(game.AddedById);
                if (game.AddedBy == null)
                {
                    throw new DribblyObjectNotFoundException($"Unable to find account with ID {game.AddedById}.");
                }
            }


            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            System.Diagnostics.Debug.WriteLine("**** GetGame execution time: " + ts.TotalSeconds.ToString());

            return game;
        }

        public async Task<DTO.GameTeam> GetGameTeamAsync(long gameId, long teamId)
        {
            TeamModel team = await _teamsRepository.Get(t => t.Id == teamId,
                $"{nameof(TeamModel.Members)}.{nameof(TeamMembershipModel.Account)}.{nameof(AccountModel.User)}," +
                $"{nameof(TeamModel.Members)}.{nameof(TeamMembershipModel.Account)}.{nameof(AccountModel.ProfilePhoto)}," +
                $"{nameof(TeamModel.Logo)}")
                .SingleOrDefaultAsync();

            GameModel game = _context.Games.SingleOrDefault(g => g.Id == gameId);

            if (team != null)
            {
                team.Members = team.Members.Where(m => m.DateLeft == null).ToList();
                var teamDto = new DTO.GameTeam(team, gameId);
                List<Task> playerTasks = new List<Task>();
                foreach (var member in teamDto.Players)
                {
                    var shots = _shotsRepository.Get(s => s.PerformedById == member.Id && s.GameId == gameId && !s.IsMiss);
                    var fouls = await _context.MemberFouls.Where(f => f.PerformedById == member.Id && f.GameId == gameId && f.TeamId == teamId).ToListAsync();
                    if (shots.Count() > 0)
                    {
                        int? points = await shots.SumAsync(s => s.Points);
                        member.Points = points ?? 0;
                        member.Fouls = fouls.Count();
                        member.HasFouledOut = member.Fouls >= game.PersonalFoulLimit;
                        member.IsEjected = fouls.Count(f => f.IsTechnical) >= game.TechnicalFoulLimit;
                    }
                }

                return teamDto;
            }

            return null;
        }

        public async Task StartGameAsync(StartGameInputModel input)
        {
            GameModel game = await _context.Games.SingleOrDefaultAsync(g => g.Id == input.GameId);
            var currentAccountId = _securityUtility.GetAccountId();
            if (game.AddedById == currentAccountId)
            {
                if (game.Status == GameStatusEnum.WaitingToStart)
                {
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {

                            game.Status = GameStatusEnum.Started;
                            game.Start = input.StartedAt;
                            game.RemainingTimeUpdatedAt = input.StartedAt;
                            game.IsLive = true;
                            await _commonService.AddUserGameActivity(UserActivityTypeEnum.StartGame, game.Id);
                            await _context.SaveChangesAsync();

                            if (input.Jumpball != null)
                            {
                                _gameEventsRepo.Add(input.Jumpball);
                                _context.SaveChanges();
                            }

                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                else
                {
                    throw new DribblyInvalidOperationException("Attempted starting a game with a status of " + game.Status,
                        friendlyMessageKey: "app.Error_Common_InvalidOperationTryReload");
                }
            }
            else
            {
                throw new DribblyForbiddenException
                    (String.Format("Unauthorized user attempted to start game. Account ID: {0}, Game ID: {1}", currentAccountId, game.Id));
            }
        }

        public async Task<UpsertShotResultModel> RecordShotAsync(ShotDetailsInputModel input)
        {
            GameModel game = await _gameRepo.Get(g => g.Id == input.Shot.GameId,
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
                    result.Team1Score = game.Team1Score;
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
                            TotalRebounds = reboundedBy.Rebounds
                        };
                        var reboundersTeam = game.Team1.TeamId == input.Rebound.TeamId ? game.Team1 : game.Team2;
                        reboundersTeam.Rebounds++;
                        await _context.SaveChangesAsync();
                    }

                    transaction.Commit();

                    return result;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

        }

        public async Task<AddGameModalModel> GetAddGameModalAsync(long courtId)
        {
            return new AddGameModalModel
            {
                CourtChoice = await _commonService.GetChoiceItemModelAsync
                (courtId, EntityTypeEnum.Court)
            };
        }

        public async Task<AddGameModalModel> GetAddGameModalAsync(GetAddGameModalInputModel input)
        {
            var result = new AddGameModalModel();
            if (input.TournamentId.HasValue)
            {
                var tournament = await _context.Tournaments.SingleOrDefaultAsync(t => t.Id == input.TournamentId);
                result.OverrideSettings(tournament);
                if (tournament.DefaultCourtId.HasValue)
                {
                    result.CourtChoice = await _commonService.GetChoiceItemModelAsync(tournament.DefaultCourtId, EntityTypeEnum.Court);
                }
            }
            else if (input.CourtId.HasValue)
            {
                result.CourtChoice = await _commonService.GetChoiceItemModelAsync(input.CourtId, EntityTypeEnum.Court);
            }

            return result;
        }

        public async Task EndGameAsync(long gameId, long winningTeamId)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                try
                {
                    GameModel game = await _context.Games
                    .Include(g => g.Team1.Players.Select(p => p.TeamMembership))
                    .Include(g => g.Team2.Players.Select(p => p.TeamMembership))
                    .SingleOrDefaultAsync(g => g.Id == gameId);

                    if ((game.Team1Score > game.Team2Score && winningTeamId != game.Team1Id)
                        || (game.Team2Score > game.Team1Score && winningTeamId != game.Team2Id))
                    {
                        throw new DribblyInvalidOperationException("Wrong winning team was provided.");
                    }

                    game.WinningTeamId = winningTeamId;
                    game.Team1.Won = game.Team1.Id == winningTeamId;
                    game.Team2.Won = game.Team2.Id == winningTeamId;
                    game.Status = GameStatusEnum.Finished;
                    game.End = DateTime.UtcNow;

                    await _context.SaveChangesAsync();

                    var allPlayers = game.Team1.Players.Concat(game.Team2.Players);

                    #region Update Player Stats
                    foreach (var p in allPlayers)
                    {
                        #region Overall Stats

                        p.Won = game.WinningTeamId == p.GameTeamId;
                        PlayerStatsModel stats = await _context.PlayerStats
                            .SingleOrDefaultAsync(s => s.AccountId == p.AccountId);

                        if (stats == null)
                        {
                            stats = new PlayerStatsModel(p.AccountId);
                            _context.SetEntityState(stats, EntityState.Added);
                        }

                        List<GamePlayerModel> allGameStats = await _context.GamePlayers
                            .Include(g=>g.TeamMembership).Include(g=>g.Game)
                            .Where(gp => gp.AccountId == p.AccountId && gp.Won.HasValue).ToListAsync();
                        allGameStats.Add(p); //add this game because this will not be included above as it's Won field is still null

                        stats.GP = allGameStats.Count();
                        stats.PPG = allGameStats.Average(s => s.Points);
                        stats.RPG = allGameStats.Average(s => s.Rebounds);
                        stats.APG = allGameStats.Average(s => s.Assists);
                        stats.FGP = allGameStats.Average(s => s.FGM.DivideBy(s.FGA));
                        stats.ThreePP = allGameStats.Average(s => s.ThreePM.DivideBy(s.ThreePA));
                        stats.FTP = allGameStats.Average(s => s.FTM.DivideBy(s.FTA));
                        stats.BPG = allGameStats.Average(s => s.Blocks);
                        stats.MPG = (allGameStats.Average(s => s.PlayTimeMs) / 60000);
                        stats.PlayTimeMs = allGameStats.Sum(s => s.PlayTimeMs);
                        stats.LastGameId = gameId;
                        stats.OverallScore = (stats.PPG / 34.5) + (stats.APG / 10.2) + (stats.RPG / 14.1) + (stats.BPG / 3.1) + (stats.PlayTimeMs / 2520000);

                        #endregion

                        #region Team Stats
                        var teammemberShip = _context.TeamMembers
                            .Single(m=>m.MemberAccountId == p.AccountId && m.TeamId == p.TeamMembership.TeamId);
                        teammemberShip.UpdateStats(allGameStats.Where(s => s.TeamMembership.TeamId == p.TeamMembership.TeamId));
                        teammemberShip.SetOverallScore();
                        #endregion

                        #region Tournament Stats

                        if (game.TournamentId.HasValue)
                        {
                            var tournamentStats = await _context.TournamentPlayers
                                .SingleOrDefaultAsync(t => t.AccountId == p.AccountId && t.TournamentId == game.TournamentId);
                            if (tournamentStats == null)
                            {
                                tournamentStats = new TournamentPlayerModel()
                                {
                                    AccountId = p.AccountId,
                                    TournamentId = game.TournamentId.Value,
                                    JerseyNo = p.JerseyNo
                                };
                                _context.TournamentPlayers.Add(tournamentStats);
                            }
                            var tournamentGames = allGameStats.Where(g => g.Game.TournamentId == game.TournamentId).ToList();

                            tournamentStats.UpdateStats(tournamentGames);
                            tournamentStats.SetOverallScore();
                        }

                        #endregion
                    }
                    #endregion

                    #region Update Team Stats

                    foreach (var team in new List<GameTeamModel> { game.Team1, game.Team2 })
                    {
                        #region Overall Stats
                        var overallStats = await _context.TeamStats.SingleOrDefaultAsync(s => s.TeamId == team.TeamId);

                        if (overallStats == null)
                        {
                            overallStats = new TeamStatsModel() { TeamId = team.TeamId };
                            _context.TeamStats.Add(overallStats);
                        }
                        var allStats = await _context.GameTeams.Where(g => g.TeamId == team.TeamId
                                                && g.Won.HasValue) //means the game is finished
                                                .ToListAsync();
                        overallStats.UpdateStats(allStats);
                        overallStats.SetOverallScore();
                        #endregion

                        #region Tournament Stats
                        if (game.TournamentId.HasValue)
                        {
                            var tournamentStats = await _context.TournamentTeams.SingleOrDefaultAsync(t => t.TeamId == team.TeamId && t.TournamentId == game.TournamentId);
                            if (tournamentStats == null)
                            {
                                tournamentStats = new TournamentTeamModel()
                                {
                                    TeamId = team.TeamId,
                                    TournamentId = game.TournamentId.Value
                                };
                                _context.TournamentTeams.Add(tournamentStats);
                            }
                            var tournamentGames = await _context.GameTeams
                                .Where(g => g.TeamId == team.TeamId && g.Game.TournamentId == game.TournamentId
                                                    && g.Won.HasValue) //means the game is finished
                                                    .ToListAsync();
                            tournamentStats.UpdateStats(tournamentGames);
                            tournamentStats.SetOverallScore();
                        }
                        #endregion
                    }
                    #endregion

                    Update(game);

                    _context.SaveChanges();
                    tx.Commit();
                    await _commonService.AddUserGameActivity(UserActivityTypeEnum.EndGame, game.Id);
                }
                catch (Exception)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public async Task<bool> CurrentUserIsGameManagerAsync(long gameId)
        {
            var accountId = _securityUtility.GetAccountId().Value;
            return await _context.Games.AnyAsync(g => g.Id == gameId && g.AddedById == accountId);
        }

        public async Task UpdateStatusAsync(long gameId, GameStatusEnum toStatus)
        {
            GameModel game = await _context.Games.Include(g => g.Tournament)
                .SingleOrDefaultAsync(g => g.Id == gameId);
            var currentAccountId = _securityUtility.GetAccountId();
            if (game.AddedById == currentAccountId || (game.Tournament?.AddedById == currentAccountId))
            {
                #region Starting Game
                if (toStatus == GameStatusEnum.Started)
                {
                    if (game.Status == GameStatusEnum.WaitingToStart)
                    {
                        game.Status = GameStatusEnum.Started;
                        game.Start = DateTime.UtcNow;
                        await _commonService.AddUserGameActivity(UserActivityTypeEnum.StartGame, game.Id);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        throw new DribblyInvalidOperationException("Attempted starting a game with a status of " + game.Status,
                            friendlyMessageKey: "app.Error_Common_InvalidOperationTryReload");
                    }
                }
                #endregion
                #region Finish Game
                else if (toStatus == GameStatusEnum.Finished)
                {
                    if (game.Status == GameStatusEnum.Started)
                    {
                        game.Status = GameStatusEnum.Finished;
                        game.End = DateTime.UtcNow;
                        await _commonService.AddUserGameActivity(UserActivityTypeEnum.EndGame, game.Id);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        throw new DribblyInvalidOperationException("Attempted ending a game with a status of " + game.Status,
                            friendlyMessageKey: "app.Error_Common_InvalidOperationTryReload");
                    }
                }
                #endregion
                #region Reset Game
                else if (toStatus == GameStatusEnum.WaitingToStart)
                {
                    if (game.Status != GameStatusEnum.WaitingToStart)
                    {
                        game.Status = GameStatusEnum.WaitingToStart;
                        game.End = DateTime.UtcNow;
                        await _commonService.AddUserGameActivity(UserActivityTypeEnum.EndGame, game.Id);
                        await _context.SaveChangesAsync();
                    }
                }
                #endregion
                #region Cancel Game
                else if (toStatus == GameStatusEnum.Cancelled)
                {
                    if (game.Status == GameStatusEnum.Finished)
                    {
                        throw new DribblyInvalidOperationException("Attempted cancel an already finished game.",
                            friendlyMessageKey: "app.Error_CancelGame_AlreadyFinished");
                    }
                    else if (game.Status == GameStatusEnum.Cancelled)
                    {
                        throw new DribblyInvalidOperationException("Attempted cancel an already canceled game.",
                            friendlyMessageKey: "app.Error_CancelGame_AlreadyCanceled");
                    }
                    else
                    {
                        game.Status = GameStatusEnum.Cancelled;
                        await _commonService.AddUserGameActivity(UserActivityTypeEnum.CancelGame, game.Id);
                        await _context.SaveChangesAsync();
                    }
                }
                #endregion
                #region Delete Game
                else if (toStatus == GameStatusEnum.Deleted)
                {
                    if (game.Status == GameStatusEnum.Finished)
                    {
                        throw new DribblyInvalidOperationException("Attempted to delete an already finished game.",
                            friendlyMessageKey: "app.Error_DeleteGame_AlreadyFinished");
                    }
                    else if (game.Status == GameStatusEnum.Started)
                    {
                        throw new DribblyInvalidOperationException("Attempted to delete an already started game.",
                            friendlyMessageKey: "app.Error_DeleteGame_AlreadyStarted");
                    }
                    else
                    {
                        game.Status = GameStatusEnum.Deleted;
                        game.EntityStatus = EntityStatusEnum.Deleted;
                        await _commonService.AddUserGameActivity(UserActivityTypeEnum.CancelGame, game.Id);
                        await _context.SaveChangesAsync();
                    }
                }
                #endregion
            }
            else
            {
                throw new DribblyForbiddenException
                    (String.Format("Unauthorized user attempted to start game. Account ID: {0}, Game ID: {1}", currentAccountId, game.Id));
            }
        }

        public async Task<GameModel> AddGameAsync(AddGameInputModel input)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    GameModel game = input.ToGameModel();
                    game.AddedById = _securityUtility.GetAccountId().Value;
                    game.Status = GameStatusEnum.WaitingToStart;
                    game.EntityStatus = EntityStatusEnum.Active;
                    game.DateAdded = DateTime.UtcNow;
                    if (game.IsTimed)
                    {
                        game.RemainingTime = game.RegulationPeriodDuration * 60 * 1000;
                        game.IsLive = false;
                        game.RemainingShotTime = game.DefaultShotClockDuration * 1000;
                    }
                    Add(game);
                    _context.SaveChanges();

                    #region Assign Team1 and Players
                    var team1 = new Model.Entities.GameTeamModel
                    {
                        DateAdded = DateTime.UtcNow,
                        TeamId = input.Team1Id,
                        GameId = game.Id,
                        TimeoutsLeft = game.TotalTimeoutLimit
                    };
                    _context.GameTeams.Add(team1);
                    _context.SaveChanges();

                    var team1Players = await _context.TeamMembers.Include(m => m.Account)
                        .Where(m => m.TeamId == input.Team1Id && !m.DateLeft.HasValue)
                        .ToListAsync();
                    foreach (var p in team1Players)
                    {
                        _context.GamePlayers.Add(new Model.Entities.GamePlayerModel
                        {
                            DateAdded = DateTime.UtcNow,
                            PlayerId = p.Id,
                            AccountId = p.MemberAccountId,
                            GameTeamId = team1.Id,
                            GameId = game.Id
                        });
                    }
                    _context.SaveChanges();
                    #endregion

                    #region Assign Team2 and Players
                    var team2 = new Model.Entities.GameTeamModel
                    {
                        DateAdded = DateTime.UtcNow,
                        TeamId = input.Team2Id,
                        GameId = game.Id,
                        TimeoutsLeft = game.TotalTimeoutLimit
                    };
                    _context.GameTeams.Add(team2);
                    _context.SaveChanges();

                    _context.IndexedEntities.Add(new IndexedEntityModel(game));
                    _context.SaveChanges();

                    var team2Players = await _context.TeamMembers.Include(m => m.Account)
                        .Where(m => m.TeamId == input.Team2Id && !m.DateLeft.HasValue)
                        .ToListAsync();
                    foreach (var p in team2Players)
                    {
                        _context.GamePlayers.Add(new Model.Entities.GamePlayerModel
                        {
                            DateAdded = DateTime.UtcNow,
                            PlayerId = p.Id,
                            AccountId = p.MemberAccountId,
                            GameTeamId = team2.Id,
                            GameId = game.Id
                        });
                    }
                    _context.SaveChanges();
                    #endregion

                    game.Team1Id = team1.Id;
                    game.Team2Id = team2.Id;
                    _context.SaveChanges();

                    transaction.Commit();
                    await _commonService.AddUserGameActivity(UserActivityTypeEnum.AddGame, game.Id);
                    NotificationTypeEnum Type = game.AddedById == _securityUtility.GetAccountId().Value ?
                        NotificationTypeEnum.NewGameForOwner :
                        NotificationTypeEnum.NewGameForBooker;
                    await _notificationsRepo.TryAddAsync(new NewGameNotificationModel
                    {
                        GameId = game.Id,
                        BookedById = game.AddedById,
                        ForUserId = Type == NotificationTypeEnum.NewGameForBooker ? game.AddedById :
                        (await _courtsRepo.GetOwnerId(game.CourtId)),
                        DateAdded = DateTime.UtcNow,
                        Type = Type
                    });

                    return _context.Games
                        .Include(g => g.Team1.Players.Select(p => p.TeamMembership.Account.User))
                        .Include(g => g.Team1.Team.Logo)
                        .Include(g => g.Team2.Players.Select(p => p.TeamMembership.Account.User))
                        .Include(g => g.Team2.Team.Logo)
                        .Single(g => g.Id == game.Id);
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public async Task SetTimeoutsLeftAsync(long gameTeamId, int timeoutsLeft)
        {
            var gameTeam = await _context.GameTeams.SingleOrDefaultAsync(t => t.Id == gameTeamId);
            // TODO: add validations

            gameTeam.TimeoutsLeft = timeoutsLeft;
            await _context.SaveChangesAsync();
        }

        public async Task SetTeamFoulCountAsync(long gameTeamId, int foulCount)
        {
            var gameTeam = await _context.GameTeams.SingleOrDefaultAsync(t => t.Id == gameTeamId);
            // TODO: add validations

            gameTeam.TeamFoulCount = foulCount;
            await _context.SaveChangesAsync();
        }

        public async Task SetBonusStatusAsync(long gameTeamId, bool isInBonus)
        {
            var gameTeam = await _context.GameTeams.SingleOrDefaultAsync(t => t.Id == gameTeamId);
            // TODO: add validations

            gameTeam.IsInBonus = isInBonus;
            await _context.SaveChangesAsync();
        }

        public async Task<RecordTimeoutResultModel> RecordTimeoutAsync(RecordTimeoutInputModel input)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = new RecordTimeoutResultModel
                    {
                        Type = input.Type,
                        TeamId = input.TeamId
                    };

                    GameEventModel timeout = new GameEventModel(GameEventTypeEnum.Timeout)
                    {
                        GameId = input.GameId,
                        TeamId = input.TeamId,
                        Period = input.Period,
                        ClockTime = input.ClockTime,
                        DateAdded = DateTime.UtcNow
                    };

                    if (!(input.Type == TimeoutTypeEnum.Official))
                    {
                        var gameTeam = await _context.GameTeams
                            .SingleOrDefaultAsync(t => t.TeamId == input.TeamId && t.GameId == input.GameId);

                        // TODO: add gameTeam null error handling

                        if (input.Type == TimeoutTypeEnum.Full)
                        {
                            gameTeam.FullTimeoutsUsed++;
                        }
                        else if (input.Type == TimeoutTypeEnum.Short)
                        {
                            gameTeam.ShortTimeoutsUsed++;
                        }

                        if (gameTeam.TimeoutsLeft > 0)
                        {
                            gameTeam.TimeoutsLeft--;
                        }

                        await _context.SaveChangesAsync();
                        result.TimeoutsLeft = gameTeam.TimeoutsLeft;
                        result.FullTimeoutsUsed = gameTeam.FullTimeoutsUsed;
                        result.ShortTimeoutsUsed = gameTeam.ShortTimeoutsUsed;
                    }

                    _context.GameEvents.Attach(timeout);
                    _context.SetEntityState(timeout, EntityState.Added);
                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    return result;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task SetNextPossessionAsync(long gameId, int nextPossession)
        {
            // TODO: add validations
            GameModel game = await _dbSet.SingleOrDefaultAsync(g => g.Id == gameId);
            game.NextPossession = nextPossession;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGameResultAsync(GameResultModel result)
        {
            GameModel game = GetById(result.GameId);
            game.Team1Score = result.Team1Score;
            game.Team2Score = result.Team2Score;
            game.WinningTeamId = result.WinningTeamId;

            if (game.Status == GameStatusEnum.Started && game.WinningTeamId.HasValue)
            {
                await UpdateStatusAsync(game.Id, GameStatusEnum.Finished);
            }

            Update(game);

            _context.SaveChanges();
            await _commonService.AddUserGameActivity(UserActivityTypeEnum.UpdateGameResult, game.Id);
        }

        public async Task AdvancePeriodAsync(long gameId, int period, int remainingTime)
        {
            GameModel game = await _dbSet.SingleOrDefaultAsync(g => g.Id == gameId);
            game.CurrentPeriod = period;
            game.RemainingTime = remainingTime;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRemainingTimeAsync(UpdateGameTimeRemainingInput input)
        {
            GameModel game = await _dbSet.SingleOrDefaultAsync(g => g.Id == input.GameId);
            if (!game.RemainingTimeUpdatedAt.HasValue || game.RemainingTimeUpdatedAt < input.UpdatedAt)
            {
                game.RemainingTime = input.TimeRemaining;
                game.RemainingTimeUpdatedAt = input.UpdatedAt;
                game.RemainingShotTime = input.ShotTimeRemaining;
                game.IsLive = input.IsLive;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<GameModel> UpdateGameAsync(UpdateGameModel input)
        {
            GameModel game = await _dbSet.Include(g => g.Court).SingleOrDefaultAsync(g => g.Id == input.Id);
            if (input.ToStatus.HasValue)
            {
                game.Status = input.ToStatus.Value;
            }

            game.Start = input.Start;
            game.End = input.End;
            game.Title = input.Title;
            game.CourtId = input.CourtId;
            //game.Team1Id = input.Team1Id;
            //game.Team2Id = input.Team2Id;
            game.IsTimed = input.IsTimed;
            if (game.IsTimed && game.Status == GameStatusEnum.WaitingToStart)
            {
                game.RemainingTime = 12 * 60 * 1000; //12mins
                game.DefaultShotClockDuration = input.DefaultShotClockDuration;
                game.OffensiveRebondShotClockDuration = input.OffensiveRebondShotClockDuration;
                game.IsLive = false;
                game.RemainingShotTime = game.DefaultShotClockDuration * 1000;
            }


            Update(game);
            bool isUpdatedByBooker = game.AddedById == _securityUtility.GetAccountId().Value;
            NotificationTypeEnum Type = isUpdatedByBooker ?
                NotificationTypeEnum.GameUpdatedForOwner :
                NotificationTypeEnum.GameUpdatedForBooker;
            await _notificationsRepo.TryAddAsync(new UpdateGameNotificationModel
            {
                GameId = game.Id,
                UpdatedById = isUpdatedByBooker ? game.AddedById : game.Court.OwnerId,
                ForUserId = Type == NotificationTypeEnum.GameUpdatedForBooker ? game.AddedById :
                game.Court.OwnerId,
                DateAdded = DateTime.UtcNow,
                Type = Type
            });
            _context.SaveChanges();
            await _commonService.AddUserGameActivity(UserActivityTypeEnum.UpdateGame, game.Id);
            return await GetGame(game.Id);
        }

        public async Task UpdateLineupAsync(UpdateLineupInputModel input)
        {
            var gameTeam = await _context.GameTeams
                .Include(t => t.Players)
                .SingleAsync(t => t.TeamId == input.TeamId && t.GameId == input.GameId);

            foreach (var player in gameTeam.Players)
            {
                player.IsInGame = input.GamePlayerIds.Contains(player.Id);
            }

            await _context.SaveChangesAsync();
        }
    }
    public interface IGamesService
    {
        IEnumerable<GameModel> GetAll();

        Task<GameModel> GetGame(long id);

        Task<IEnumerable<GameModel>> GetGamesAsync(GetGamesFilterModel filter);

        Task<AddGameModalModel> GetAddGameModalAsync(long courtId);

        Task<AddGameModalModel> GetAddGameModalAsync(GetAddGameModalInputModel input);

        Task AdvancePeriodAsync(long gameId, int period, int remainingTime);

        Task<GameModel> AddGameAsync(AddGameInputModel input);

        Task<bool> CurrentUserIsGameManagerAsync(long gameId);

        Task EndGameAsync(long gameId, long winningTeamId);

        Task UpdateStatusAsync(long gameId, GameStatusEnum toStatus);

        Task<GameModel> UpdateGameAsync(UpdateGameModel Game);

        Task UpdateGameResultAsync(GameResultModel result);

        Task StartGameAsync(StartGameInputModel input);

        Task<DTO.GameTeam> GetGameTeamAsync(long gameId, long teamId);

        Task<UpsertShotResultModel> RecordShotAsync(ShotDetailsInputModel input);

        Task UpdateRemainingTimeAsync(UpdateGameTimeRemainingInput input);

        Task<RecordTimeoutResultModel> RecordTimeoutAsync(RecordTimeoutInputModel input);

        Task SetNextPossessionAsync(long gameId, int nextPossession);

        Task SetTimeoutsLeftAsync(long gameTeamId, int timeoutsLeft);

        Task SetTeamFoulCountAsync(long gameTeamId, int foulCount);

        Task SetBonusStatusAsync(long gameTeamId, bool isInBonus);

        Task UpdateLineupAsync(UpdateLineupInputModel input);
    }
}