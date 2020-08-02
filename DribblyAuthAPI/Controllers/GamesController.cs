using Dribbly.Model.Games;
using Dribbly.Service.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Games")]
    public class GamesController : BaseController
    {
        private IGamesService _service = null;

        public GamesController(IGamesService service) : base()
        {
            _service = service;
        }

        //GETs
        [HttpGet]
        [Route("GetAllGames")]
        public IEnumerable<GameModel> GetAllGames()
        {
            return _service.GetAll();
        }

        [HttpGet]
        [Route("GetGame/{id}")]
        public async Task<GameModel> GetGame(long id)
        {
            return await _service.GetGame(id);
        }

        [HttpPost, Authorize]
        [Route("UpdateGame")]
        public void UpdateGame([FromBody] GameModel model)
        {
            _service.UpdateGame(model);
        }

        [HttpPost, Authorize]
        [Route("BookGame")]
        public GameModel Register([FromBody] GameModel model)
        {
            return _service.BookGame(model);
        }
    }
}
