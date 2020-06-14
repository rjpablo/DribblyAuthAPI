using Dribbly.Service.Models;
using Dribbly.Service.Models.Courts;
using Dribbly.Service.Services;
using System.Collections.Generic;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Settings")]
    public class SettingsController : BaseController
    {
        private ISettingsService _service = null;

        public SettingsController(ISettingsService service) : base()
        {
            _service = service;
        }

        //GETs
        [HttpGet]
        [Route("GetInitialSettings")]
        public IEnumerable<SettingModel> GetInitialSettings()
        {
            return _service.GetInitialSettings();
        }

        [HttpGet]
        [Route("GetValue/{key}")]
        public string GetValue(string key)
        {
            return _service.GetValue(key);
        }

    }
}
