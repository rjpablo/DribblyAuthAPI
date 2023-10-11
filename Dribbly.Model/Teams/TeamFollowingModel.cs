using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Teams
{
    public class TeamFollowingModel
    {
        [Key]
        [Column(Order = 1)]
        public long TeamId { get; set; }

        [Key]
        [Column(Order = 2)]
        public long FollowedById { get; set; }
    }
}
