using Dribbly.Model.Courts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.UserActivities
{
    [Table("UserCourtActivities")]
    public class UserCourtActivityModel : UserActivityModel
    {
        [ForeignKey(nameof(Court))]
        public long CourtId { get; set; }

        public CourtModel Court { get; set; }

    }
}
