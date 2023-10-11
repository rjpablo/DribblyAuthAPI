using Dribbly.Model.Seasons;
using Dribbly.Service.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Seasons")]
    [Authorize]
    public class SeasonsController : BaseController
    {
        private ISeasonsService _service = null;

        public SeasonsController(ISeasonsService service) : base()
        {
            _service = service;
        }

        //POSTS
        [HttpPost]
        [Route("AddSeason")]
        public async Task<SeasonModel> AddSeason([FromBody] SeasonModel input)
        {
            return await _service.AddSeasonAsync(input);
        }
    }
}
