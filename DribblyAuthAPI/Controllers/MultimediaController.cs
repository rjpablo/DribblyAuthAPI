using Dribbly.Core.Models;
using Dribbly.Service.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Multimedia")]
    [Authorize]
    public class MultimediaController : BaseController
    {
        private readonly IMultimediaService _multimediaService;

        public MultimediaController(IMultimediaService multimediaService)
        {
            _multimediaService = multimediaService;
        }

        [HttpPost]
        [Route("Upload")]
        public async Task<IEnumerable<MultimediaModel>> Upload()
        {
            return await _multimediaService.UploadAsync();
        }
    }
}
