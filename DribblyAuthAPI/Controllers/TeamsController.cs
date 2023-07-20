using Dribbly.Core.Models;
using Dribbly.Model.Courts;
using Dribbly.Model.DTO;
using Dribbly.Model.Teams;
using Dribbly.Service.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Teams")]
    public class TeamsController : BaseController
    {
        private ITeamsService _service = null;

        public TeamsController(ITeamsService service) : base()
        {
            _service = service;
        }

        //GETs
        [HttpGet]
        [Route("GetAllTeams")]
        public IEnumerable<TeamModel> GetAllTeams()
        {
            return _service.GetAll();
        }

        [HttpGet]
        [Route("GetTeam/{id}")]
        public async Task<TeamModel> GetTeam(long id)
        {
            return await _service.GetTeamAsync(id);
        }

        [HttpGet]
        [Route("GetCurrentMembers/{teamId}")]
        public async Task<IEnumerable<TeamMembershipModel>> GetCurrentMembers(long teamId)
        {
            return await _service.GetCurrentMembersAsync(teamId);
        }

        [HttpGet, Authorize]
        [Route("GetJoinRequests/{teamId}")]
        public async Task<IEnumerable<JoinTeamRequestModel>> GetJoinRequests(long teamId)
        {
            return await _service.GetJoinRequestsAsync(teamId);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetTeamViewerData/{teamId}")]
        public async Task<TeamViewerDataModel> GetTeamViewerData(long teamId)
        {
            return await _service.GetTeamViewerDataAsync(teamId);
        }

        [HttpGet]
        [Route("GetUserTeamRelation/{teamId}")]
        public async Task<UserTeamRelationModel> GetUserTeamRelation(long teamId)
        {
            return await _service.GetUserTeamRelationAsync(teamId);
        }

        //POSTs

        [HttpPost, Authorize]
        [Route("CancelJoinRequest/{teamId}")]
        public async Task CancelJoinRequest(long teamId)
        {
            await _service.CancelJoinRequestAsync(teamId);
        }

        [HttpPost, Authorize]
        [Route("LeaveTeam/{teamId}")]
        public async Task<UserTeamRelationModel> LeaveTeam(long teamId)
        {
            return await _service.LeaveTeamAsync(teamId);
        }

        [HttpPost, AllowAnonymous]
        [Route("GetTopTeams")]
        public async Task<IEnumerable<TeamStatsViewModel>> GetTopTeams([FromBody] PagedGetInputModel input)
        {
            return await _service.GetTopTeamsAsync(input);
        }

        [HttpPost, Authorize]
        [Route("UpdateTeam")]
        public async Task UpdateTeam([FromBody] TeamModel model)
        {
            await _service.UpdateTeamAsync(model);
        }

        [HttpPost, Authorize]
        [Route("AddTeam")]
        public async Task<TeamModel> AddTeam([FromBody] TeamModel model)
        {
            return await _service.AddTeamAsync(model);
        }

        [HttpPost, Authorize]
        [Route("UploadLogo/{teamId}")]
        public async Task<PhotoModel> UploadLogo(long teamId)
        {
            return await _service.UploadLogoAsync(teamId);
        }

        [HttpPost, Authorize]
        [Route("JoinTeam")]
        public async Task<UserTeamRelationModel> JoinTeam(JoinTeamRequestInputModel input)
        {
            return await _service.JoinTeamAsync(input);
        }

        [HttpPost]
        [Route("RemoveMember/{teamId}/{membershipId}")]
        public async Task RemoveMember(long teamId, long membershipId)
        {
            await _service.RemoveMemberAsync(teamId, membershipId);
        }

        [HttpPost, Authorize]
        [Route("ProcessJoinRequest")]
        public async Task ProcessJoinRequest([FromBody] ProcessJoinTeamRequestInputModel input)
        {
            await _service.ProcessJoinRequestAsync(input);
        }
    }
}
