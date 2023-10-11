using Dribbly.Service.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Permissions")]
    public class PermissionsController : BaseController
    {
        private IPermissionsService _service = null;

        public PermissionsController(IPermissionsService service) : base()
        {
            _service = service;
        }

        //GETs
        [HttpGet]
        [Route("GetUserPermissionNames/{userId}")]
        public async Task<IEnumerable<string>> GetUserPermissionNames(string userId)
        {
            return await _service.GetUserPermissionNamesAsync(userId);
        }
    }
}
