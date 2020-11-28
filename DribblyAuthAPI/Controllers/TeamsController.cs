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
            return await _service.GetTeam(id);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetTeamViewerData/{teamId}")]
        public async Task<TeamViewerDataModel> GetTeamViewerData(long teamId)
        {
            return await _service.GetTeamViewerDataAsync(teamId);
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
    }
}
