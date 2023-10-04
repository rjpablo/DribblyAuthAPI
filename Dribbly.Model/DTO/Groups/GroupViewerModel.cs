using Dribbly.Model.Entities.Groups;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.DTO.Groups
{
    [NotMapped]
    public class GroupViewerModel : GroupModel
    {
        public bool IsAdmin { get; set; }
        public GroupUserRelationship UserRelationship { get; set; }
        public GroupViewerModel(GroupModel source, long? forAccountId)
        {
            {
                DateAdded = source.DateAdded;
                Id = source.Id;
                Name = source.Name;
                LogoId = source.LogoId;
                AddedById = source.AddedById;
                EntityStatus = source.EntityStatus;
                Description = source.Description;
                Logo = source.Logo;
                AddedBy = source.AddedBy;
                Members = source.Members;
                IsAdmin = forAccountId == source.AddedById;
                UserRelationship = new GroupUserRelationship(source, forAccountId);
            }
        }
    }
}
