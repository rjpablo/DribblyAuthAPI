using Dribbly.Model.Bookings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public interface IBookingsService
    {
        IEnumerable<BookingModel> GetAll();

        Task<BookingModel> GetBooking(long id);

        Task<BookingModel> AddBookingAsync(BookingModel Booking);

        Task UpdateBookingAsync(BookingModel Booking);
    }
}