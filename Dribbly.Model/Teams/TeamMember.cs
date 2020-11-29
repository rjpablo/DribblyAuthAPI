using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Service.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Teams
{
    [Table("TeamMemberships")]
    public class TeamMembershipModel : BaseEntityModel
    {
        [ForeignKey(nameof(Team))]
        public long TeamId { get; set; }
        [ForeignKey(nameof(Member))]
        public long MemberAccountId { get; set; }
        /// <summary>
        /// If null, it means that the player is still a member of the team
        /// </summary>
        public DateTime? DateLeft { get; set; }
        public PlayerPositionEnum Position { get; set; }
        public AccountModel Member { get; set; }
        public TeamModel Team { get; set; }
    }
}
