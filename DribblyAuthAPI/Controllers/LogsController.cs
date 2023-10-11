using Dribbly.Model.Logs;
using Dribbly.Service.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Logs")]
    public class LogsController : BaseController
    {
        private ILogsService _service = null;

        public LogsController(ILogsService service) : base()
        {
            _service = service;
        }

        [HttpPost]
        [Route("LogClientError")]
        public async Task LogClientError(ClientLogModel log)
        {
            await _service.LogClientError(log);
        }
    }
}
