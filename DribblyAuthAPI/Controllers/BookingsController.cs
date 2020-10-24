using Dribbly.Model.Bookings;
using Dribbly.Service.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Bookings")]
    public class BookingsController : BaseController
    {
        private IBookingsService _service = null;

        public BookingsController(IBookingsService service) : base()
        {
            _service = service;
        }

        //GETs
        [HttpGet]
        [Route("GetAllBookings")]
        public IEnumerable<BookingModel> GetAllBookings()
        {
            return _service.GetAll();
        }

        [HttpGet]
        [Route("GetBooking/{id}")]
        public async Task<BookingModel> GetBooking(long id)
        {
            return await _service.GetBooking(id);
        }

        [HttpPost, Authorize]
        [Route("UpdateBooking")]
        public async Task UpdateBooking([FromBody] BookingModel model)
        {
            await _service.UpdateBookingAsync(model);
        }

        [HttpPost, Authorize]
        [Route("AddBooking")]
        public async Task<BookingModel> AddBooking([FromBody] BookingModel model)
        {
            return await _service.AddBookingAsync(model);
        }
    }
}
