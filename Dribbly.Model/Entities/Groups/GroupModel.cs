using Dribbly.Core.Enums;
using Dribbly.Core.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities.Groups
{
    [Table("Groups")]
    public class GroupModel : BaseEntityModel, IIndexedEntity
    {
        public string Name { get; set; }
        [ForeignKey(nameof(Logo))]
        public long? LogoId { get; set; }
        public string IconUrl { get { return Logo?.Url; } }
        [ForeignKey(nameof(AddedBy))]
        public long AddedById { get; set; }
        public EntityStatusEnum EntityStatus { get; set; }
        public EntityTypeEnum EntityType => EntityTypeEnum.Group;
        public string Description { get; set; }
        public MultimediaModel Logo { get; set; }
        public AccountModel AddedBy { get; set; }
        public ICollection<GroupMemberModel> Members { get; set; } = new List<GroupMemberModel>();
        public ICollection<JoinGroupRequest> JoinRequests { get; set; } = new List<JoinGroupRequest>();
    }
}
