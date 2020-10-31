using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Account;
using Dribbly.Model.Games;
using Dribbly.Model.Notifications;
using Dribbly.Service.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;

namespace Dribbly.Service.Services
{
    public class GamesService : BaseEntityService<GameModel>, IGamesService
    {
        IAuthContext _context;
        HttpContextBase _httpContext;
        ISecurityUtility _securityUtility;
        IFileService _fileService;
        IAccountRepository _accountRepo;
        INotificationsRepository _notificationsRepo;
        ICourtsRepository _courtsRepo;

        public GamesService(IAuthContext context,
            HttpContextBase httpContext,
            ISecurityUtility securityUtility,
            IAccountRepository accountRepo,
            IFileService fileService,
            INotificationsRepository notificationsRepo,
            ICourtsRepository courtsRepo) : base(context.Games)
        {
            _context = context;
            _httpContext = httpContext;
            _securityUtility = securityUtility;
            _accountRepo = accountRepo;
            _fileService = fileService;
            _notificationsRepo = notificationsRepo;
            _courtsRepo = courtsRepo;
        }

        public IEnumerable<GameModel> GetAll()
        {
            return All();
        }

        public async Task<GameModel> GetGame(long id)
        {
            GameModel game = await _dbSet.Include(g => g.Court).SingleOrDefaultAsync(g => g.Id == id);
            if (!string.IsNullOrWhiteSpace(game.AddedById))
            {
                game.AddedBy = await _accountRepo.GetAccountBasicInfo(game.AddedById);
            }
            return game;
        }

        public async Task<GameModel> AddGameAsync(AddGameInputModel input)
        {
            GameModel game = input.ToGameModel();
            var currentUserId = _securityUtility.GetUserId();
            game.AddedById = currentUserId;
            game.Status = Enums.GameStatusEnum.WaitingToStart;
            Add(game);
            _context.SaveChanges();
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
        }
    }
}