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

        [ForeignKey("Event")]
        public long EventId { get; set; }

        public EventModel Event { get; set; }

    }
}