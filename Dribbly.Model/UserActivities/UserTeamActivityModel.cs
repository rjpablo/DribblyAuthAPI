using Dribbly.Model.Teams;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.UserActivities
{
    [Table("UserTeamActivities")]
    public class UserTeamActivityModel : UserActivityModel
    {
        [ForeignKey(nameof(Team))]
        public long? TeamId { get; set; }
        public TeamModel Team { get; set; }

    }
}
