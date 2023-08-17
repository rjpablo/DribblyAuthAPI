using Dribbly.Model.Entities;
using Dribbly.Model.Fouls;
using Dribbly.Model.GameEvents;
using Dribbly.Model.Play;
using Dribbly.Service.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/GameEvents")]
    [Authorize]
    public class GameEventsController : BaseController
    {
        private IGameEventsService _service = null;

        public GameEventsController(IGameEventsService service) : base()
        {
            _service = service;
        }

        //POSTS
        [HttpPost]
        [Route("UpsertFoul")]
        public async Task<UpsertFoulResultModel> UpsertFoul([FromBody] MemberFoulModel input)
        {
            return await _service.UpsertFoulAsync(input);
        }

        [HttpPost]
        [Route("Upsert")]
        public async Task<GameEventModel> UpsertAsync([FromBody] GameEventModel gameEvent)
        {
            return await _service.UpsertAsync(gameEvent);
        }

        [HttpPost]
        [Route("RecordShot")]
        public async Task<UpsertShotResultModel> RecordShot(ShotDetailsInputModel input)
        {
            return await _service.RecordShotAsync(input);
        }

        [HttpPost]
        [Route("RecordTurnover")]
        public async Task RecordTurnover(GameEventModel turnover)
        {
            await _service.RecordTurnoverAsync(turnover);
        }

        [HttpPost]
        [Route("Update")]
        public async Task<UpdateGameEventResultModel> Update([FromBody] UpdateGameEventInputModel input)
        {
            return await _service.UpdateAsync(input);
        }

        [HttpPost]
        [Route("Delete/{gameEventId}")]
        public async Task<UpdateGameEventResultModel> Delete(long gameEventId)
        {
            return await _service.DeleteAsync(gameEventId);
        }
    }
}
