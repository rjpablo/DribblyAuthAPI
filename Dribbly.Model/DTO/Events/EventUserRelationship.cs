using Dribbly.Model.Entities.Events;
using System.Linq;

namespace Dribbly.Model.DTO.Events
{
    public class EventUserRelationship
    {
        public bool IsCurrentMember { get; set; }
        public bool IsAdmin { get; set; }
        public bool HasJoinRequest { get; set; }

        public EventUserRelationship(EventModel group, long? accountId)
        {
            IsAdmin = accountId == group.AddedById;
            HasJoinRequest = group.Attendees.Where(a=>!a.IsApproved).Any(r => r.AccountId == accountId);
            IsCurrentMember = group.Attendees.Where(a => a.IsApproved).Any(m => m.AccountId == accountId);
        }
    }
}
