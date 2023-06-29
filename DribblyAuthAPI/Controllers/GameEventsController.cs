using Dribbly.Model.Entities;
using Dribbly.Model.Fouls;
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
    }
}
