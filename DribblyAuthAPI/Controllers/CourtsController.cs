using DribblyAuthAPI.Models.Courts;
using DribblyAuthAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Courts")]
    public class CourtsController : BaseController
    {
        private CourtsRepository _repo = null;

        public CourtsController():base()
        {
            _repo = new CourtsRepository();
        }

        [HttpGet]
        [Route("GetAllCourts")]
        public IEnumerable<CourtModel> GetAllCourts()
        {
            return _repo.GetAll();
        }

    }
}
