using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Teams
{
    [Table("TeamMemberships")]
    public class TeamMembershipModel : BaseEntityModel, IIndexedEntity, ITeamMemberListItem
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

        public EntityTypeEnum EntityType => Member.EntityType;

        public string Name => Member.Name;

        public string IconUrl => Member.IconUrl;

        public EntityStatusEnum EntityStatus { get => Member.EntityStatus; }

        public string Description => Member.Description;
        public int JerseyNo { get; set; }

        [NotMapped]
        public bool IsCurrentMember => !DateLeft.HasValue;

        [NotMapped]
        public bool IsFormerMember => DateLeft.HasValue;

        [NotMapped]
        public bool HasPendingJoinRequest => false;

        [NotMapped]
        public string PrimaryPhotoUrl => IconUrl;
    }
}
