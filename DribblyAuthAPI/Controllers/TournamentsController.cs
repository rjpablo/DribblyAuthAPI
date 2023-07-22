using Dribbly.Model.Tournaments;
using Dribbly.Service.Services;
using System.Collections.Generic;
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
        [HttpGet, AllowAnonymous]
        [Route("GetTournamentviewer/{leagueId}")]
        public async Task<TournamentViewerModel> GetTournamentviewer(long leagueId)
        {
            return await _service.GetTournamentViewerAsync(leagueId);
        }

        
        [HttpPost, AllowAnonymous]
        [Route("GetNew")]
        public async Task<IEnumerable<TournamentModel>> GetNew([FromBody] GetTournamentsInputModel input)
        {
            return await _service.GetNewAsync(input);
        }

        //POSTS
        [HttpPost]
        [Route("AddTournament")]
        public async Task<TournamentModel> AddTournament([FromBody] TournamentModel input)
        {
            return await _service.AddTournamentAsync(input);
        }

        //POSTS
        [HttpPost]
        [Route("JoinTournament/{tournamentId}/{teamId}")]
        public async Task JoinTournament(long tournamentId, long teamId)
        {
            await _service.JoinTournamentAsync(tournamentId, teamId);
        }
    }
}
