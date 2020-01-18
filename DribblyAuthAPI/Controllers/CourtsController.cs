using DribblyAuthAPI.Models;
using DribblyAuthAPI.Models.Courts;
using DribblyAuthAPI.Repositories;
using DribblyAuthAPI.Services;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Courts")]
    public class CourtsController : BaseController
    {
        private ICourtsService _service = null;

        public CourtsController() : base()
        {
            _service = new CourtsService(new AuthContext());
        }

        //GETs
        [HttpGet]
        [Route("GetAllCourts")]
        public IEnumerable<CourtModel> GetAllCourts()
        {
            return _service.GetAll();
        }

        // POSTs
        [HttpPost]
        [Route("Register")]
        public void Register([FromBody] CourtModel model)
        {
            _service.Register(model);
        }
    }
}
