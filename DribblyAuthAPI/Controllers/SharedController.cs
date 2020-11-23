using Dribbly.Model.Shared;
using Dribbly.Service.Services.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Shared")]
    [Authorize]
    public class SharedController : BaseController
    {
        private readonly ICommonService _commonService;

        public SharedController(ICommonService commonService) : base()
        {
            _commonService = commonService;
        }

        [HttpPost]
        [Route("GetTypeAheadSuggestions")]
        public async Task<IEnumerable<ChoiceItemModel<long>>> GetTypeAheadSuggestions
            ([FromBody] GetTypeAheadSuggestionsInputModel input)
        {
            return await _commonService.GetTypeAheadSuggestionsAsync(input);
        }
    }
}
