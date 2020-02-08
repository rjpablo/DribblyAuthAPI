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

        public CourtsController(ICourtsService service) : base()
        {
            _service = service;
        }

        //GETs
        [HttpGet]
        [Route("GetAllCourts")]
        public IEnumerable<CourtModel> GetAllCourts()
        {
            return _service.GetAll();
        }

        [HttpGet]
        [Route("GetCourt/{id}")]
        public CourtModel GetCourt(long id)
        {
            return _service.GetCourt(id);
        }

        // POSTs
        [HttpPost]
        [Route("UpdateCourt")]
        public void UpdateCourt([FromBody] CourtModel model)
        {
            _service.UpdateCourt(model);
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
