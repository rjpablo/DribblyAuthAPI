using Dribbly.Model.Games;
using Dribbly.Service.Enums;
using Dribbly.Service.Services;
using System;
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

        [HttpGet]
        [Route("GetGameTeam/{gameId}/{teamId}")]
        public async Task<Dribbly.Service.DTO.GameTeam> GetGameTeam(long gameId, long teamId)
        {
            return await _service.GetGameTeamAsync(gameId, teamId);
        }

        [HttpGet]
        [Route("GetAddGameModal/{courtId}")]
        public async Task<AddGameModalModel> GetAddGameModal(long courtId)
        {
            return await _service.GetAddGameModalAsync(courtId);
        }

        // POSTs

        [HttpPost]
        [Route("EndGame/{gameId}/{winningTeamId}")]
        public async Task EndGame(long gameId, long winningTeamId)
        {
            await _service.EndGameAsync(gameId, winningTeamId);
        }

        [HttpPost, Authorize]
        [Route("UpdateGame")]
        public async Task<GameModel> UpdateGame([FromBody] UpdateGameModel model)
        {
            return await _service.UpdateGameAsync(model);
        }

        [HttpPost, Authorize]
        [Route("AdvancePeriod/{gameId}/{period}/{remainingTime}")]
        public async Task AdvancePeriod(long gameId, int period, int remainingTime)
        {
            await _service.AdvancePeriodAsync(gameId, period, remainingTime);
        }

        [HttpPost, Authorize]
        [Route("UpdateRemainingTime")]
        public async Task UpdateRemainingTime([FromBody] UpdateGameTimeRemainingInput input)
        {
            await _service.UpdateRemainingTimeAsync(input);
        }

        [HttpPost, Authorize]
        [Route("RecordShot")]
        public async Task<GameModel> RecordShot([FromBody] ShotModel shot)
        {
            return await _service.RecordShotAsync(shot);
        }

        [HttpPost, Authorize]
        [Route("UpdateGameResult")]
        public async Task UpdateGameResult(GameResultModel result)
        {
            await _service.UpdateGameResultAsync(result);
        }

        [HttpPost, Authorize]
        [Route("AddGame")]
        public async Task<GameModel> AddGame([FromBody] AddGameInputModel model)
        {
            return await _service.AddGameAsync(model);
        }

        [HttpPost, Authorize]
        [Route("UpdateStatus/{gameId}/{toStatus}")]
        public async Task UpdateStatus(long gameId, GameStatusEnum toStatus)
        {
            await _service.UpdateStatusAsync(gameId, toStatus);
        }
    }
}
