using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Enums;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Teams
{
    [Table("JoinTeamRequests")]
    public class JoinTeamRequestModel : BaseEntityModel, IIndexedEntity, ITeamMemberListItem
    {
        [ForeignKey(nameof(Team))]
        public long TeamId { get; set; }
        [ForeignKey(nameof(Member))]
        public long MemberAccountId { get; set; }
        public PlayerPositionEnum Position { get; set; }
        public JoinTeamRequestStatus Status { get; set; }
        public AccountModel Member { get; set; }
        public TeamModel Team { get; set; }

        public EntityTypeEnum EntityType => Member.EntityType;

        public string Name => Member.Name;

        public string IconUrl => Member.IconUrl;

        public EntityStatusEnum EntityStatus { get => Member.EntityStatus; }

        public string Description => Member.Description;

        [NotMapped]
        public bool IsCurrentMember => false;

        [NotMapped]
        public bool IsFormerMember => false;

        [NotMapped]
        public bool HasPendingJoinRequest => Status == JoinTeamRequestStatus.Pending;

        [NotMapped]
        public string PrimaryPhotoUrl => IconUrl;
    }
}
