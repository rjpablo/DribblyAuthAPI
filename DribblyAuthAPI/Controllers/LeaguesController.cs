using Dribbly.Model.Leagues;
using Dribbly.Service.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Leagues")]
    [Authorize]
    public class LeaguesController : BaseController
    {
        private ILeaguesService _service = null;

        public LeaguesController(ILeaguesService service) : base()
        {
            _service = service;
        }

        //POSTS
        [HttpPost]
        [Route("AddLeague")]
        public async Task<LeagueModel> AddLeague([FromBody] LeagueModel input)
        {
            return await _service.AddLeagueAsync(input);
        }
    }
}
