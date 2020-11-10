using Dribbly.Test.Model;
using Dribbly.Test.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Test")]
    [Authorize]
    public class TestsController : BaseController
    {
        private ITestService _testService;

        public TestsController(ITestService testService) : base()
        {
            _testService = testService;
        }

        //GETs
        [HttpGet, AllowAnonymous]
        [Route("GetUserActivities")]
        public async Task<IEnumerable<TestUserActivityViewModel>> GetUserActivities()
        {
            return await _testService.GetUserActivities();
        }
    }
}
