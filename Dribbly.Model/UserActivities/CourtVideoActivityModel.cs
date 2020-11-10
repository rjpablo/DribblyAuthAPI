using Dribbly.Model.Courts;
using Dribbly.Model.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.UserActivities
{
    [Table("CourtVideoActivities")]
    public class CourtVideoActivityModel : UserActivityModel
    {
        [ForeignKey(nameof(Video))]
        public long VideoId { get; set; }
        [ForeignKey(nameof(Court))]
        public long CourtId { get; set; }

        public VideoModel Video { get; set; }
        public CourtModel Court { get; set; }

    }
}
