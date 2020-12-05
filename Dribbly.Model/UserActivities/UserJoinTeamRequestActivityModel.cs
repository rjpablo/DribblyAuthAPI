using Dribbly.Model.Teams;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.UserActivities
{
    [Table("UserJoinTeamRequestActivities")]
    public class UserJoinTeamRequestActivityModel : UserActivityModel
    {
        [ForeignKey(nameof(Request))]
        public long RequestId { get; set; }
        public JoinTeamRequestModel Request { get; set; }
    }
}
