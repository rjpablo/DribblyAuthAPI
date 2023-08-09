using Dribbly.Core.Models;
using Dribbly.Identity.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Courts;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.UI.WebControls;

namespace Dribbly.Model.Teams
{
    [Table("Teams")]
    public class TeamModel : BaseEntityModel, IIndexedEntity
    {
        public string Name { get; set; }
        [MaxLength(15)]
        public string ShortName { get; set; }
        [ForeignKey(nameof(Logo))]
        public long? LogoId { get; set; }
        public string IconUrl { get { return Logo?.Url; } }
        public long AddedById { get; set; }
        public bool IsOpen { get; set; }
        /// <summary>
        /// References AspNetUsers.Id
        /// </summary>
        public long ManagedById { get; set; }
        public EntityStatusEnum EntityStatus { get; set; }
        [ForeignKey(nameof(HomeCourt))]
        public long? HomeCourtId { get; set; }
        [NotMapped]
        public EntityTypeEnum EntityType => EntityTypeEnum.Team;
        [NotMapped]
        public string Description { get; set; }
        public CourtModel HomeCourt { get; set; }
        public PhotoModel Logo { get; set; }
        public AccountBasicInfoModel AddedBy { get; set; }
        public AccountBasicInfoModel ManagedBy { get; set; }
        public virtual ICollection<TeamMembershipModel> Members { get; set; }
    }
}
