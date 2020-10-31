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
        public async Task UpdateGame([FromBody] GameModel model)
        {
            await _service.UpdateGameAsync(model);
        }

        [HttpPost, Authorize]
        [Route("AddGame")]
        public async Task<GameModel> AddGame([FromBody] AddGameInputModel model)
        {
            return await _service.AddGameAsync(model);
        }
    }
}
