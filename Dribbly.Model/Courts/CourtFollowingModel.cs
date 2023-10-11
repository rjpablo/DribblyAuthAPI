using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Shared
{
    //[Table("CourtFollowings")]
    public class CourtFollowingModel
    {
        [Key]
        [Column(Order = 1)]
        public long CourtId { get; set; }

        [Key]
        [Column(Order = 2)]
        public long FollowedById { get; set; }

    }
}