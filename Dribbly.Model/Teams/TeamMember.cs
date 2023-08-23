using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Entities;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Teams
{
    [Table("TeamMemberships")]
    public class TeamMembershipModel : BaseStatsSummaryModel, IIndexedEntity, ITeamMemberListItem
    {
        [ForeignKey(nameof(Team))]
        public long TeamId { get; set; }
        [ForeignKey(nameof(Account))]
        public long MemberAccountId { get; set; }
        /// <summary>
        /// If null, it means that the player is still a member of the team
        /// </summary>
        public DateTime? DateLeft { get; set; }
        public PlayerPositionEnum Position { get; set; }
        public AccountModel Account { get; set; }
        [JsonIgnore]
        public TeamModel Team { get; set; }

        public EntityTypeEnum EntityType => Account.EntityType;

        public string Name => Account?.Name;
        [NotMapped]
        public string Username => Account?.Username;
        public string FirstName => Account?.FirstName;
        public string LastName => Account?.LastName;

        public string IconUrl => Account?.IconUrl;

        public EntityStatusEnum EntityStatus { get => Account.EntityStatus; }

        public string Description => Account.Description;
        public int JerseyNo { get; set; }

        [NotMapped]
        public bool IsCurrentMember => !DateLeft.HasValue;

        [NotMapped]
        public bool IsFormerMember => DateLeft.HasValue;

        [NotMapped]
        public bool HasPendingJoinRequest => false;

        [NotMapped]
        public string PrimaryPhotoUrl => IconUrl;
        [NotMapped]
        public PhotoModel ProfilePhoto { get => Account?.ProfilePhoto; }
    }
}
