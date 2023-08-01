using Dribbly.Model.Courts;
using Dribbly.Model.DTO;
using Dribbly.Model.Entities;
using Dribbly.Model.Shared;
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

        [HttpGet]
        [Route("IsCurrentUserManager/{tournamentId}")]
        public async Task<bool> IsCurrentUserManager(long tournamentId)
        {
            return await _service.IsCurrentUserManagerAsync(tournamentId);
        }

        [HttpPost]
        [Route("UpdateTournamentSettings")]
        public async Task<TournamentModel> UpdateTournamentSettings(UpdateTournamentSettingsModel settings)
        {
            return await _service.UpdateTournamentSettingsAsync(settings);
        }

        [HttpGet]
        [Route("GetTournamentTeamsAsChoices/{tournamentId}/{stageId}")]
        public async Task<IEnumerable<ChoiceItemModel<long>>> GetTournamentTeamsAsChoicesAsync(long tournamentId, long? stageId)
        {
            return await _service.GetTournamentTeamsAsChoicesAsync(tournamentId, stageId);
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
        [Route("SetStageTeams")]
        public async Task<TournamentStageModel> SetStageTeams([FromBody] SetStageTeamsInputModel input)
        {
            return await _service.SetStageTeamsAsync(input);
        }

        [HttpPost]
        [Route("DeleteStage/{stageId}")]
        public async Task DeleteStage(long stageId)
        {
            await _service.DeleteStageAsync(stageId);
        }

        [HttpPost]
        [Route("SetTeamBracket/{teamId}/{stageId}/{bracketId?}")]
        public async Task SetTeamBracket(long teamId, long stageId, long? bracketId)
        {
            await _service.SetTeamBracket(teamId, stageId, bracketId);
        }

        [HttpPost]
        [Route("DeleteStageBracket/{bracketId?}")]
        public async Task DeleteStageBracket(long bracketId)
        {
            await _service.DeleteStageBracketAsync(bracketId);
        }

        [HttpPost]
        [Route("AddStageBracket/{bracketName}/{stageId}")]
        public async Task<StageBracketModel> AddStageBracket(string bracketName, long stageId)
        {
            return await _service.AddStageBracketAsync(bracketName, stageId);
        }

        [HttpPost]
        [Route("RenameStage/{stageId}/{name}")]
        public async Task RenameStage(long stageId, string name)
        {
            await _service.RenameStageAsync(stageId, name);
        }

        [HttpPost]
        [Route("AddTournamentStage")]
        public async Task<TournamentStageModel> AddTournamentStage(AddTournamentStageInputModel input)
        {
            return await _service.AddTournamentStageAsync(input);
        }

        [HttpPost, AllowAnonymous]
        [Route("GetTopTeams")]
        public async Task<IEnumerable<TeamStatsViewModel>> GetTopTeams(GetTournamentTeamsInputModel input)
        {
            return await _service.GetTopTeamsAsync(input);
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
        [Route("UpdateLogo/{tournamentId}")]
        public async Task<PhotoModel> UpdateLogo(long tournamentId)
        {
            return await _service.UpdateLogoAsync(tournamentId);
        }

        [HttpPost]
        [Route("RemoveTournamentTeam/{tournamentId}/{teamId}")]
        public async Task RemoveTournamentTeam(long tournamentId, long teamId)
        {
            await _service.RemoveTournamentTeamAsync(tournamentId, teamId);
        }
    }
}
