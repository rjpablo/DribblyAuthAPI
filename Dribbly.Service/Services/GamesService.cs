using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Dribbly.Core.Utilities;
using Dribbly.Service.Models;
using Dribbly.Service.Models.Games;

namespace Dribbly.Service.Services
{
    public class GamesService : BaseEntityService<GameModel>, IGamesService
    {
        IAuthContext _context;
        HttpContextBase _httpContext;
        ISecurityUtility _securityUtility;
        IFileService _fileService;

        public GamesService(IAuthContext context,
            HttpContextBase httpContext,
            ISecurityUtility securityUtility,
            IFileService fileService) : base(context.Games)
        {
            _context = context;
            _httpContext = httpContext;
            _securityUtility = securityUtility;
            _fileService = fileService;
        }

        public IEnumerable<GameModel> GetAll()
        {
            return All();
        }

        public GameModel GetGame(long id)
        {
            return GetById(id);
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