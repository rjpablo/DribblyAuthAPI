using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Account;
using Dribbly.Model.Games;
using Dribbly.Service.Repositories;
using System.Collections.Generic;
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

        public GamesService(IAuthContext context,
            HttpContextBase httpContext,
            ISecurityUtility securityUtility,
            IAccountRepository accountRepo,
            IFileService fileService) : base(context.Games)
        {
            _context = context;
            _httpContext = httpContext;
            _securityUtility = securityUtility;
            _accountRepo = accountRepo;
            _fileService = fileService;
        }

        public IEnumerable<GameModel> GetAll()
        {
            return All();
        }

        public async Task<GameModel> GetGame(long id)
        {
            var game = GetById(id);
            if (!string.IsNullOrWhiteSpace(game.BookedById))
            {
                game.BookedBy = new AccountsChoicesItemModel(await _accountRepo.GetAccountById(game.BookedById));
            }
            return game;
        }

        public GameModel BookGame(GameModel Game)
        {
            Game.AddedBy = _securityUtility.GetUserId();
            Add(Game);
            _context.SaveChanges();
            return Game;
        }

        public void UpdateGame(GameModel Game)
        {
            Update(Game);
            _context.SaveChanges();
        }
    }
}