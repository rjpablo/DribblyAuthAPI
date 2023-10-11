using Dribbly.Model.Entities.Groups;
using System.Linq;

namespace Dribbly.Model.DTO.Groups
{
    public class GroupUserRelationship
    {
        public bool IsCurrentMember { get; set; }
        public bool IsAdmin { get; set; }
        public bool HasJoinRequest { get; set; }

        public GroupUserRelationship(GroupModel group, long? accountId)
        {
            IsAdmin = accountId == group.AddedById;
            HasJoinRequest = group.JoinRequests.Any(r => r.RequestorId == accountId);
            IsCurrentMember = group.Members.Any(m => m.AccountId == accountId);
        }
    }
}
