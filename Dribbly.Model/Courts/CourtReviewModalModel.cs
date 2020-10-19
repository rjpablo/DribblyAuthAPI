using Dribbly.Model.Bookings;
using Dribbly.Model.Shared;

namespace Dribbly.Model.Courts
{
    public class CourtReviewModalModel : ReviewModel
    {
        public virtual CourtModel Court { get; set; }
        public BookingModel Booking { get; set; }
    }
}