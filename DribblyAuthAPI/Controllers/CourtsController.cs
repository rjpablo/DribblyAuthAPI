using DribblyAuthAPI.Models;
using DribblyAuthAPI.Models.Courts;
using DribblyAuthAPI.Models.Games;
using DribblyAuthAPI.Repositories;
using DribblyAuthAPI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<IEnumerable<CourtModel>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        [HttpGet]
        [Route("GetCourt/{id}")]
        public async Task<CourtModel> GetCourtAsync(long id)
        {
            return await _service.GetCourtAsync(id);
        }

        [HttpGet]
        [Route("GetCourtPhotos/{courtId}")]
        public IEnumerable<PhotoModel> GetCourtPhotos(long courtId)
        {
            return _service.GetCourtPhotos(courtId);
        }

        [HttpGet]
        [Route("GetCourtGames/{courtId}")]
        public IEnumerable<GameModel> GetCourtGames(long courtId)
        {
            return _service.GetCourtGames(courtId);
        }

        [HttpPost]
        [Route("FindCourts")]
        public async Task<IEnumerable<CourtModel>> FindCourts
            ([FromBody]CourtSearchInputModel input)
        {
            return await _service.FindCourtsAsync(input);
        }

        // POSTs
        [HttpPost, Authorize]
        [Route("UpdateCourtPhoto/{courtId}")]
        public void UpdateCourtPhoto(long courtId)
        {
            _service.UpdateCourtPhoto(courtId);
        }

        [HttpPost, Authorize]
        [Route("AddCourtPhotos/{courtId}")]
        public IEnumerable<PhotoModel> AddCourtPhotos(long courtId)
        {
            return _service.AddPhotos(courtId);
        }

        [HttpPost, Authorize]
        [Route("UpdateCourt")]
        public void UpdateCourt([FromBody] CourtModel model)
        {
            _service.UpdateCourt(model);
        }

        [HttpPost, Authorize]
        [Route("Register")]
        public long Register([FromBody] CourtModel model)
        {
            return _service.Register(model);
        }

        [HttpPost, Authorize]
        [Route("DeletePhoto/{courtId}/{photoId}")]
        public async Task DeletePhoto(long courtId, long photoId)
        {
            await _service.DeletePhotoAsync(courtId, photoId);
        }
    }
}
