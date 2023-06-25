using Dribbly.Core.Exceptions;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Account;
using Dribbly.Model.Games;
using Dribbly.Model.Notifications;
using Dribbly.Model.Play;
using Dribbly.Model.Teams;
using Dribbly.Service.Enums;
using Dribbly.Service.Repositories;
using Dribbly.Service.Services.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        private readonly IMemberFoulsRepository _memberFoulsRepository;
        private readonly IShotsRepository _shotsRepository;

        public GamesService(IAuthContext context,
            ISecurityUtility securityUtility,
            IAccountRepository accountRepo,
            IFileService fileService,
            INotificationsRepository notificationsRepo,
            ICourtsRepository courtsRepo,
            ICommonService commonService,
            ITeamsRepository teamsRepository) : base(context.Games)
        {
            _context = context;
            _securityUtility = securityUtility;
            _accountRepo = accountRepo;
            _fileService = fileService;
            _notificationsRepo = notificationsRepo;
            _courtsRepo = courtsRepo;
            _commonService = commonService;
            _teamsRepository = teamsRepository;
            _memberFoulsRepository = new MemberFoulsRepository(context);
            _shotsRepository = new ShotsRepository(context);
        }

        public IEnumerable<GameModel> GetAll()
        {
            return All();
        }

        public async Task<GameModel> GetGame(long id)
        {
            GameModel game = await _dbSet.Where(g => g.Id == id).Include(g => g.Court).Include(g => g.Court.PrimaryPhoto)
                .Include(g => g.Team1).Include(g => g.Team1.Logo)
                .Include(g => g.Team2).Include(g => g.Team2.Logo)
                .SingleOrDefaultAsync();
            if (game != null)
            {
                game.AddedBy = await _accountRepo.GetAccountBasicInfo(game.AddedById);
                if (game.AddedBy == null)
                {
                    throw new DribblyObjectNotFoundException($"Unable to find account with ID {game.AddedById}.");
                }
            }
            return game;
        }

        public async Task<DTO.GameTeam> GetGameTeamAsync(long gameId, long teamId)
        {
            TeamModel team = await _teamsRepository.Get(t => t.Id == teamId,
                $"{nameof(TeamModel.Members)}.{nameof(TeamMembershipModel.Member)}.{nameof(AccountModel.User)}," +
                $"{nameof(TeamModel.Members)}.{nameof(TeamMembershipModel.Member)}.{nameof(AccountModel.ProfilePhoto)}," +
                $"{nameof(TeamModel.Logo)}")
                .SingleOrDefaultAsync();

            if (team != null)
            {
                team.Members = team.Members.Where(m => m.DateLeft == null).ToList();
                var teamDto = new DTO.GameTeam(team, gameId);
                List<Task> playerTasks = new List<Task>();
                foreach (var member in teamDto.Players)
                {
                    var shots = _shotsRepository.Get(s => s.TakenById == member.Id && s.GameId == gameId && !s.IsMiss);
                    var foulCount = await _context.MemberFouls.CountAsync(f => f.PerformedById == member.Id && f.GameId == gameId && f.TeamId == teamId);
                    if (shots.Count() > 0)
                    {
                        int? points = await shots.SumAsync(s => s.Points);
                        member.Points = points ?? 0;
                        member.Fouls = foulCount;
                    }
                }

                return teamDto;
            }

            return null;
        }

        public async Task<UpsertShotResultModel> RecordShotAsync(ShotDetailsInputModel input)
        {
            GameModel game = GetById(input.Shot.GameId);

            if (game == null)
            {
                throw new DribblyObjectNotFoundException($"A game with ID {input.Shot.Game.Id} does not exist.");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = new UpsertShotResultModel();
                    if (!input.Shot.IsMiss)
                    {
                        if (input.Shot.TeamId == game.Team1Id)
                        {
                            game.Team1Score += input.Shot.Points;
                        }
                        else if (input.Shot.TeamId == game.Team2Id)
                        {
                            game.Team2Score += input.Shot.Points;
                        }

                        input.Shot.Game = null;
                        _shotsRepository.Add(input.Shot);

                        Update(game);
                        await _context.SaveChangesAsync();

                        result.TotalPoints = await _context.Shots
                            .Where(s => s.TakenById == input.Shot.TakenById && s.GameId == input.Shot.GameId && !s.IsMiss)
                            .SumAsync(s => s.Points);
                        result.Game = game;
                    }

                    if (input.WithFoul)
                    {
                        result.FoulResult = await _memberFoulsRepository.UpsertFoul(input.Foul);
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

        public async Task EndGameAsync(long gameId, long winningTeamId)
        {
            GameModel game = await _context.Games.SingleOrDefaultAsync(g => g.Id == gameId);
            if ((game.Team1Score > game.Team2Score && winningTeamId != game.Team1Id)
                || (game.Team2Score > game.Team1Score && winningTeamId != game.Team2Id))
            {
                throw new DribblyInvalidOperationException("Wrong winning team was provided.");
            }

            game.WinningTeamId = winningTeamId;
            game.Status = GameStatusEnum.Finished;
            game.End = DateTime.UtcNow;

            Update(game);

            _context.SaveChanges();
            await _commonService.AddUserGameActivity(UserActivityTypeEnum.EndGame, game.Id);
        }

        public async Task UpdateStatusAsync(long gameId, GameStatusEnum toStatus)
        {
            GameModel game = await _context.Games.SingleOrDefaultAsync(g => g.Id == gameId);
            var currentUserId = _securityUtility.GetUserId();
            if (game.AddedById == currentUserId)
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
                    (String.Format("Unauthorized user attempted to start game. User ID: {0}, Game ID: {1}", currentUserId, game.Id));
            }
        }

        public async Task<GameModel> AddGameAsync(AddGameInputModel input)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    GameModel game = input.ToGameModel();
                    var currentUserId = _securityUtility.GetUserId();
                    AccountModel account = await _accountRepo.GetAccountByIdentityId(currentUserId.Value);
                    game.AddedById = account.Id;
                    game.Status = GameStatusEnum.WaitingToStart;
                    game.EntityStatus = EntityStatusEnum.Active;
                    if (game.IsTimed)
                    {
                        game.RemainingTime = 12 * 60 * 1000; //12mins
                        game.IsLive = false;
                        game.RemainingShotTime = game.DefaultShotClockDuration * 1000;
                    }

                    if (input.IsTeam1Open)
                    {
                        TeamModel team1 = new TeamModel
                        {
                            AddedById = account.Id,
                            DateAdded = DateTime.UtcNow,
                            IsOpen = true,
                            ManagedById = account.Id
                        };
                        _context.Teams.Add(team1);
                        _context.SaveChanges();
                        game.Team1Id = team1.Id;
                    }

                    if (input.IsTeam2Open)
                    {
                        TeamModel team2 = new TeamModel
                        {
                            AddedById = account.Id,
                            DateAdded = DateTime.UtcNow,
                            IsOpen = true,
                            ManagedById = account.Id
                        };
                        _context.Teams.Add(team2);
                        _context.SaveChanges();
                        game.Team2Id = team2.Id;
                    }

                    Add(game);
                    _context.SaveChanges();
                    transaction.Commit();
                    await _commonService.AddUserGameActivity(UserActivityTypeEnum.AddGame, game.Id);
                    NotificationTypeEnum Type = game.AddedById == account.Id ?
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

                    return game;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
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
            if (game.RemainingTimeUpdatedAt < input.UpdatedAt)
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
            game.Team1Id = input.Team1Id;
            game.Team2Id = input.Team2Id;
            game.IsTimed = input.IsTimed;
            if (game.IsTimed && game.Status == GameStatusEnum.WaitingToStart)
            {
                game.RemainingTime = 12 * 60 * 1000; //12mins
                game.DefaultShotClockDuration = input.DefaultShotClockDuration;
                game.OffensiveRebondShotClock = input.OffensiveRebondShotClock;
                game.IsLive = false;
                game.RemainingShotTime = game.DefaultShotClockDuration * 1000;
            }


            Update(game);
            var currentUserId = _securityUtility.GetUserId();
            AccountModel currentUserAccount = await _accountRepo.GetAccountByIdentityId(currentUserId.Value);
            bool isUpdatedByBooker = game.AddedById == currentUserAccount.Id;
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
    }
}