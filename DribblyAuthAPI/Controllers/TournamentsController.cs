using Dribbly.Model.DTO;
using Dribbly.Model.Entities;
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
                
        [HttpGet, AllowAnonymous]
        [Route("GetTournamentviewer/{leagueId}")]
        public async Task<TournamentViewerModel> GetTournamentviewer(long leagueId)
        {
            return await _service.GetTournamentViewerAsync(leagueId);
        }
                
        [HttpGet, AllowAnonymous]
        [Route("GetTournamentStages/{tournamentId}")]
        public async Task<IEnumerable<TournamentStageModel>> GetTournamentStagesAsync(long tournamentId)
        {
            return await _service.GetTournamentStagesAsync(tournamentId);
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
        
        [HttpPost]
        [Route("AddTournamentStage")]
        public async Task AddTournamentStage(AddTournamentStageInputModel input)
        {
            await _service.AddTournamentStageAsync(input);
        }
        
        [HttpPost]
        [Route("ProcessJoinRequest/{requestId}/{shouldApprove}")]
        public async Task<TeamStatsViewModel> ProcessJoinRequest(long requestId, bool shouldApprove)
        {
            return await _service.ProcessJoinRequestAsync(requestId, shouldApprove);
        }

        //POSTS
        [HttpPost]
        [Route("JoinTournament/{tournamentId}/{teamId}")]
        public async Task JoinTournament(long tournamentId, long teamId)
        {
            await _service.JoinTournamentAsync(tournamentId, teamId);
        }

        [HttpPost]
        [Route("RemoveTournamentTeam/{tournamentId}/{teamId}")]
        public async Task RemoveTournamentTeam(long tournamentId, long teamId)
        {
            await _service.RemoveTournamentTeamAsync(tournamentId, teamId);
        }
    }
}
