using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Enums;
using Dribbly.Service.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Teams
{
    [Table("JoinTeamRequests")]
    public class JoinTeamRequestModel : BaseEntityModel
    {
        [ForeignKey(nameof(Team))]
        public long TeamId { get; set; }
        [ForeignKey(nameof(Member))]
        public long MemberAccountId { get; set; }
        public PlayerPositionEnum Position { get; set; }
        public JoinTeamRequestStatus Status { get; set; }
        public AccountModel Member { get; set; }
        public TeamModel Team { get; set; }
    }
}
