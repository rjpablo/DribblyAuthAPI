using Dribbly.Model.Tournaments;
using Dribbly.Service.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Tournaments")]
    [Authorize]
    public class TournamentsController : BaseController
    {
        private ITournamentsService _service = null;

        public TournamentsController(ITournamentsService service) : base()
        {
            _service = service;
        }

        //POSTS
        [HttpGet]
        [Route("GetTournamentviewer/{leagueId}")]
        public async Task<TournamentViewerModel> GetTournamentviewer(long leagueId)
        {
            return await _service.GetTournamentViewerAsync(leagueId);
        }

        //POSTS
        [HttpPost]
        [Route("AddTournament")]
        public async Task<TournamentModel> AddTournament([FromBody] TournamentModel input)
        {
            return await _service.AddTournamentAsync(input);
        }
    }
}
