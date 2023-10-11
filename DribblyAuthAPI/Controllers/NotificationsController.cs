using Dribbly.Model.Notifications;
using Dribbly.Service.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Notifications")]
    public class NotificationsController : BaseController
    {
        private INotificationsService _service = null;

        public NotificationsController(INotificationsService service) : base()
        {
            _service = service;
        }

        //GETs
        [HttpPost, Authorize]
        [Route("GetUnviewed")]
        public async Task<IEnumerable<NotificationModel>> GetUnviewed([FromBody]DateTime? afterDate)
        {
            return await _service.GetUnviewedAsync(afterDate);
        }

        [HttpPost, Authorize]
        [Route("GetNewNofications")]
        public async Task<GetNewNotificationsResultModel> GetNewNofications([FromBody]DateTime afterDate)
        {
            return await _service.GetNewNoficationsAsync(afterDate);
        }

        [HttpGet, Authorize]
        [Route("GetUnviewedCount")]
        public async Task<UnviewedCountModel> GetUnviewedCount()
        {
            return await _service.GetUnviewedCountAsync();
        }

        // POSTs
        [HttpPost, Authorize]
        [Route("SetIsViewed/{notificationId}/{isViewed}")]
        public async Task<UnviewedCountModel> SetIsViewed(long notificationId, bool isViewed)
        {
            return await _service.SetIsViewedAsync(notificationId, isViewed);
        }

        [HttpPost, Authorize]
        [Route("GetNoficationDetails/{getCount}")]
        public async Task<IEnumerable<object>> GetNoficationDetails([FromBody] DateTime? beforeDate, int getCount = 10)
        {
            return await _service.GetNoficationDetailsAsync(beforeDate, getCount);
        }
    }
}
