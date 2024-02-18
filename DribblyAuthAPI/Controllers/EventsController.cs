using Dribbly.Core.Models;
using Dribbly.Model.DTO.Events;
using Dribbly.Model.Entities.Events;
using Dribbly.Service.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Events")]
    [Authorize]
    public class EventsController : BaseController
    {
        private IEventsService _service = null;

        public EventsController(IEventsService service) : base()
        {
            _service = service;
        }

        //GETs
        [HttpGet]
        [AllowAnonymous]
        [Route("GetEventViewerData/{eventId}")]
        public async Task<EventViewerModel> GetEventViewerData(long eventId)
        {
            return await _service.GetEventViewerData(eventId);
        }
        
        [HttpGet]
        [AllowAnonymous]
        [Route("GetEventEntity/{eventId}")]
        public async Task<IIndexedEntity> GetEventEntity(long eventId)
        {
            return await _service.GetEventEntity(eventId);
        }

        //POSTs
        [HttpPost]
        [Route("CancelJoinRequest/{eventId}")]
        public async Task CancelJoinRequest(long eventId)
        {
            await _service.CancelJoinRequest(eventId);
        }

        [HttpPost]
        [Route("RemoveAttendee/{eventId}/{accountId}")]
        public async Task RemoveAttendee(long eventId, long accountId)
        {
            await _service.RemoveAttendeeAsync(eventId, accountId);
        }

        [HttpPost]
        [Route("ProcessJoinRequest/{requestId}/{isApproved}")]
        public async Task ProcessJoinRequestAsync(long requestId, bool isApproved)
        {
            await _service.ProcessJoinRequestAsync(requestId, isApproved);
        }

        [HttpPost]
        [Route("CreateEvent")]
        public async Task<EventModel> CreateEvent(AddEditEventInputModel input)
        {
            return await _service.CreateEventAsync(input);
        }

        [HttpPost]
        [Route("JoinEvent/{eventId}")]
        public async Task JoinEvent(long eventId)
        {
            await _service.JoinEventAsync(eventId);
        }

        [HttpPost]
        [Route("LeaveEvent/{eventId}")]
        public async Task LeaveEventAsync(long eventId)
        {
            await _service.LeaveEventAsync(eventId);
        }

        [HttpPost]
        [Route("SetLogo/{eventId}")]
        public async Task<MultimediaModel> SetLogo(long eventId)
        {
            return await _service.SetLogoAsync(eventId);
        }

        [HttpPost]
        [Route("UpdateEvent")]
        public async Task<EventModel> UpdateEvent(AddEditEventInputModel input)
        {
            return await _service.UpdateEventAsync(input);
        }
    }
}
