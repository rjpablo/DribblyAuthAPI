using Dribbly.Core.Exceptions;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Games;
using Dribbly.Model.Notifications;
using Dribbly.Model.Teams;
using Dribbly.Service.Enums;
using Dribbly.Service.Repositories;
using Dribbly.Service.Services.Shared;
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

        public async Task<AddGameModalModel> GetAddGameModalAsync(long courtId)
        {
            return new AddGameModalModel
            {
                CourtChoice = await _commonService.GetChoiceItemModelAsync
                (courtId, EntityTypeEnum.Court)
            };
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
                    game.AddedById = currentUserId.Value;
                    game.Status = GameStatusEnum.WaitingToStart;
                    game.EntityStatus = EntityStatusEnum.Active;

                    if (input.IsTeam1Open)
                    {
                        TeamModel team1 = new TeamModel
                        {
                            AddedById = currentUserId.Value,
                            DateAdded = DateTime.UtcNow,
                            IsOpen = true,
                            ManagedById = currentUserId.Value
                        };
                        _context.Teams.Add(team1);
                        _context.SaveChanges();
                        game.Team1Id = team1.Id;
                    }

                    if (input.IsTeam2Open)
                    {
                        TeamModel team2 = new TeamModel
                        {
                            AddedById = currentUserId.Value,
                            DateAdded = DateTime.UtcNow,
                            IsOpen = true,
                            ManagedById = currentUserId.Value
                        };
                        _context.Teams.Add(team2);
                        _context.SaveChanges();
                        game.Team2Id = team2.Id;
                    }

                    Add(game);
                    _context.SaveChanges();
                    transaction.Commit();
                    await _commonService.AddUserGameActivity(UserActivityTypeEnum.AddGame, game.Id);
                    NotificationTypeEnum Type = game.AddedById == currentUserId ?
                        NotificationTypeEnum.NewBookingForOwner :
                        NotificationTypeEnum.NewBookingForBooker;
                    await _notificationsRepo.TryAddAsync(new NewBookingNotificationModel
                    {
                        BookingId = game.Id,
                        BookedById = game.AddedById,
                        ForUserId = Type == NotificationTypeEnum.NewBookingForBooker ? game.AddedById :
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

        public async Task UpdateGameAsync(GameModel game)
        {
            Update(game);
            var currentUserId = _securityUtility.GetUserId();
            NotificationTypeEnum Type = game.AddedById == currentUserId ?
                NotificationTypeEnum.NewBookingForOwner :
                NotificationTypeEnum.NewBookingForBooker;
            await _notificationsRepo.TryAddAsync(new NewBookingNotificationModel
            {
                BookingId = game.Id,
                BookedById = game.AddedById,
                ForUserId = Type == NotificationTypeEnum.NewBookingForBooker ? game.AddedById :
                (await _courtsRepo.GetOwnerId(game.CourtId)),
                DateAdded = DateTime.UtcNow,
                Type = Type
            });
            _context.SaveChanges();
            await _commonService.AddUserGameActivity(UserActivityTypeEnum.UpdateGame, game.Id);
        }
    }
}