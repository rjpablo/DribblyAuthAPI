using Dribbly.Core.Models;
using Dribbly.Model.DTO;
using Dribbly.Model.GameEvents;
using Dribbly.Model.Games;
using Dribbly.Model.Play;
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

        [HttpGet, Authorize]
        [Route("CanTrackGame/{gameId}")]
        public async Task<bool> CanTrackGame(long gameId)
        {
            return await _service.CanTrackGameAsync(gameId);
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

        [HttpPost]
        [Route("GetAddGameModal")]
        public async Task<AddGameModalModel> GetAddGameModalAsync([FromBody] GetAddGameModalInputModel input)
        {
            return await _service.GetAddGameModalAsync(input);
        }

        [HttpPost]
        [Route("GetGames")]
        public async Task<IEnumerable<GameModel>> GetGames(GetGamesFilterModel filter)
        {
            return await _service.GetGamesAsync(filter);
        }

        [HttpPost, Authorize]
        [Route("UpdateGame")]
        public async Task<GameModel> UpdateGame([FromBody] UpdateGameModel model)
        {
            return await _service.UpdateGameAsync(model);
        }

        [HttpPost, Authorize]
        [Route("RecordTimeout")]
        public async Task<RecordTimeoutResultModel> RecordTimeout(RecordTimeoutInputModel input)
        {
            return await _service.RecordTimeoutAsync(input);
        }

        [HttpPost, Authorize]
        [Route("UpdateLineup")]
        public async Task UpdateLineup(UpdateLineupInputModel input)
        {
            await _service.UpdateLineupAsync(input);
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
        [Route("UpdateGameResult")]
        public async Task UpdateGameResult(GameResultModel result)
        {
            await _service.UpdateGameResultAsync(result);
        }

        [HttpPost, Authorize]
        [Route("StartGame")]
        public async Task StartGame(StartGameInputModel input)
        {
            await _service.StartGameAsync(input);
        }

        [HttpPost, Authorize]
        [Route("SetTimeoutsLeft/{gameTeamId}/{timeoutsLeft}")]
        public async Task SetTimeoutsLeft(long gameTeamId, int timeoutsLeft)
        {
            await _service.SetTimeoutsLeftAsync(gameTeamId, timeoutsLeft);
        }

        [HttpPost, Authorize]
        [Route("SetBonusStatus/{gameTeamId}/{isInBonus}")]
        public async Task SetBonusStatus(long gameTeamId, bool isInBonus)
        {
            await _service.SetBonusStatusAsync(gameTeamId, isInBonus);
        }

        [HttpPost, Authorize]
        [Route("SetTeamFoulCount/{gameTeamId}/{foulCount}")]
        public async Task SetTeamFoulCount(long gameTeamId, int foulCount)
        {
            await _service.SetTeamFoulCountAsync(gameTeamId, foulCount);
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

        [HttpPost, Authorize]
        [Route("SetNextPossession/{gameId}/{nextPossession}")]
        public async Task SetNextPossession(long gameId, int nextPossession)
        {
            await _service.SetNextPossessionAsync(gameId, nextPossession);
        }

        [HttpPost, Authorize]
        [Route("SetTimekeeper/{gameId}/{timekeeperId?}")]
        public async Task<AccountModel> SetTimeKeeper(long gameId, long? timekeeperId)
        {
            return await _service.SetTimekeeperAsync(gameId, timekeeperId);
        }
    }
}
