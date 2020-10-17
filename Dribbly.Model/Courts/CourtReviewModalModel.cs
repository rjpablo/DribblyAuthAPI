using Dribbly.Model.Shared;

namespace Dribbly.Model.Courts
{
    public class CourtReviewModalModel : ReviewModel
    {
        public virtual CourtModel Court { get; set; }
        public EventModel Event { get; set; }
    }
}