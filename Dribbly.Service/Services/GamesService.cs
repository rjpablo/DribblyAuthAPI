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
            var game = await _dbSet.Include(g => g.Court).SingleOrDefaultAsync(g => g.Id == id);
            if (!string.IsNullOrWhiteSpace(game.BookedById))
            {
                game.BookedBy = await _accountRepo.GetAccountBasicInfo(game.BookedById);
                var account = await _accountRepo.GetAccountById(game.BookedById);
                if (account != null)
                {
                    game.BookedByChoice = new AccountsChoicesItemModel(account);
                }
            }
            return game;
        }

        public async Task<GameModel> BookGameAsync(GameModel Game)
        {
            var currentUserId = _securityUtility.GetUserId();
            Game.AddedBy = currentUserId;
            Add(Game);
            _context.SaveChanges();
            NotificationTypeEnum Type = Game.BookedById == currentUserId ?
                NotificationTypeEnum.GameBookedForOwner :
                NotificationTypeEnum.GameBookedForBooker;
            await _notificationsRepo.TryAddAsync(new GameBookedNotificationModel
            {
                GameId = Game.Id,
                BookedById = Game.BookedById,
                ForUserId = Type == NotificationTypeEnum.GameBookedForBooker ? Game.BookedById :
                (await _courtsRepo.GetOwnerId(Game.CourtId)),
                DateAdded = DateTime.UtcNow,
                Type = Type
            });

            return Game;
        }

        public async Task UpdateGameAsync(GameModel Game)
        {
            Update(Game);
            var currentUserId = _securityUtility.GetUserId();
            NotificationTypeEnum Type = Game.BookedById == currentUserId ?
                NotificationTypeEnum.GameBookedForOwner :
                NotificationTypeEnum.GameBookedForBooker;
            await _notificationsRepo.TryAddAsync(new GameBookedNotificationModel
            {
                GameId = Game.Id,
                BookedById = Game.BookedById,
                ForUserId = Type == NotificationTypeEnum.GameBookedForBooker ? Game.BookedById :
                (await _courtsRepo.GetOwnerId(Game.CourtId)),
                DateAdded = DateTime.UtcNow,
                Type = Type
            });
            _context.SaveChanges();
        }
    }
}