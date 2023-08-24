using Dribbly.Core.Models;
using Dribbly.Model.Bookings;
using Dribbly.Model.Courts;
using Dribbly.Model.Games;
using Dribbly.Model.Shared;
using Dribbly.Service.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IdentityModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
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
        public async Task<IEnumerable<CourtDetailsViewModel>> GetAllAsync()
        {
            return await _service.GetAllActiveAsync();
        }


        [HttpGet]
        [Route("GetCourt/{id}")]
        public async Task<CourtDetailsViewModel> GetCourtAsync(long id)
        {
            return await _service.GetCourtAsync(id);
        }
        
        [HttpGet, Authorize]
        [Route("GetCodeReviewModal/{courtId}")]
        public async Task<CourtReviewModalModel> GetCodeReviewModal(long courtId)
        {
            return await _service.GetCodeReviewModalAsync(courtId);
        }

        [HttpGet]
        [Route("GetCourtPhotos/{courtId}")]
        public IEnumerable<MultimediaModel> GetCourtPhotos(long courtId)
        {
            return _service.GetCourtPhotos(courtId);
        }

        [HttpGet]
        [Route("GetCourtVideos/{courtId}")]
        public async Task<IEnumerable<VideoModel>> GetCourtVideos(long courtId)
        {
            return await _service.GetCourtVideosAsync(courtId);
        }

        [HttpGet]
        [Route("GetCourtBookings/{courtId}")]
        public IEnumerable<BookingModel> GetCourtBookings(long courtId)
        {
            return _service.GetCourtBookings(courtId);
        }

        [HttpGet]
        [Route("GetCourtGames/{courtId}")]
        public async Task<IEnumerable<GameModel>> GetCourtGames(long courtId)
        {
            return await _service.GetCourtGamesAsync(courtId);
        }

        [HttpGet]
        [Route("GetReviews/{courtId}")]
        public async Task<IEnumerable<CourtReviewModel>> GetReviews(long courtId)
        {
            return await _service.GetReviewsAsync(courtId);
        }

        [HttpPost]
        [Route("FindCourts")]
        public async Task<IEnumerable<CourtModel>> FindCourts
            ([FromBody]CourtSearchInputModel input)
        {
            return await _service.FindActiveCourtsAsync(input);
        }

        // POSTs
        [HttpPost, Authorize]
        [Route("UpdateCourtPhoto/{courtId}")]
        public async Task UpdateCourtPhoto(long courtId)
        {
            await _service.UpdateCourtPhoto(courtId);
        }
                
        [HttpPost, Authorize]
        [Route("DeleteCourt/{courtId}")]
        public async Task DeleteCourt(long courtId)
        {
            await _service.DeleteCourtAsync(courtId);
        }

        [HttpPost, Authorize]
        [Route("AddCourtPhotos/{courtId}")]
        public async Task<IEnumerable<MultimediaModel>> AddCourtPhotos(long courtId)
        {
            return await _service.AddPhotosAsync(courtId);
        }

        [HttpPost, Authorize]
        [Route("SubmitReview")]
        public async Task SubmitReview([FromBody] CourtReviewModel review)
        {
            await _service.SubmitReviewAsync(review);
        }

        [HttpPost, Authorize]
        [Route("FollowCourt/{courtId}/{isFollowing?}")]
        public Task<FollowResultModel> FollowCourt(long courtId, bool isFollowing = true)
        {
            return _service.FollowCourtAsync(courtId, isFollowing);
        }

        [HttpPost, Authorize]
        [Route("AddCourtVideo/{courtId}")]
        public async Task<VideoModel> AddVideo(long courtId)
        {
            HttpFileCollection files = HttpContext.Current.Request.Files;
            if (files.Count > 1)
            {
                throw new BadRequestException("Tried to upload multiple videos at once.");
            }
            else if (files.Count == 0)
            {
                if (HttpContext.Current.Response.ClientDisconnectedToken.IsCancellationRequested)
                {
                    return await Task.FromResult<VideoModel>(null);
                }
                else
                {
                    throw new BadRequestException("Tried to upload a video but no file was received.");
                }
            }

            var result = await Request.Content.ReadAsMultipartAsync();

            var requestJson = await result.Contents[1].ReadAsStringAsync();
            var video = JsonConvert.DeserializeObject<VideoModel>(requestJson);

            return await _service.AddVideoAsync(courtId, video, files[0]);
        }

        [HttpPost]
        [Authorize]
        [Route("UpdateCourt")]
        public async Task UpdateCourt([FromBody] CourtModel model)
        {
            await _service.UpdateCourtAsync(model);
        }

        [HttpPost]
        [Authorize]
        [Route("UpdateCourtProperties")]
        public async Task UpdateCourtProperties([FromBody] GenericEntityUpdateInputModel input)
        {
            await _service.UpdateCourtPropertiesAsync(input);
        }

        [HttpPost, Authorize]
        [Route("Register")]
        public async Task<long> Register([FromBody] CourtModel model)
        {
            return await _service.RegisterAsync(model);
        }

        [HttpPost, Authorize]
        [Route("DeletePhoto/{courtId}/{photoId}")]
        public async Task DeletePhoto(long courtId, long photoId)
        {
            await _service.DeletePhotoAsync(courtId, photoId);
        }

        [HttpPost, Authorize]
        [Route("DeleteCourtVideo/{courtId}/{videoId}")]
        public async Task DeleteCourtVideo(long courtId, long videoId)
        {
            await _service.DeleteCourtVideoAsync(courtId, videoId);
        }
    }
}
