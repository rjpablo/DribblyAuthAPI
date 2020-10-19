using Dribbly.Model.Bookings;
using Dribbly.Model.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Courts
{
    [Table("CourtReviews")]
    public class CourtReviewModel : ReviewModel
    {
        [ForeignKey("Court"), Required]
        public long CourtId { get; set; }

        // navigation properties
        public virtual CourtModel Court { get; set; }

        [ForeignKey(nameof(Booking))]
        public long EventId { get; set; }

        public BookingModel Booking { get; set; }

    }
}