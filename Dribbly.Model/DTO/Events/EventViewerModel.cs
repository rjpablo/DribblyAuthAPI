using Dribbly.Model.Entities.Events;
using Dribbly.Model.Entities.Groups;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.DTO.Events
{
    [NotMapped]
    public class EventViewerModel : EventModel
    {
        public bool IsAdmin { get; set; }
        public EventUserRelationship UserRelationship { get; set; }
        public EventViewerModel(EventModel source, long? forAccountId)
        {
            {
                DateAdded = source.DateAdded;
                Id = source.Id;
                Title = source.Name;
                StartDate = source.StartDate;
                EndDate = source.EndDate;
                LogoId = source.LogoId;
                AddedById = source.AddedById;
                AddedBy = source.AddedBy;
                EntityStatus = source.EntityStatus;
                Description = source.Description;
                Logo = source.Logo;
                AddedBy = source.AddedBy;
                Attendees = source.Attendees;
                IsAdmin = forAccountId == source.AddedById;
                CourtId = source.CourtId;
                Court = source.Court;
                UserRelationship = new EventUserRelationship(source, forAccountId);
            }
        }
    }
}
