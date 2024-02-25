using System;

namespace Dribbly.Model.DTO.Events
{
    public class AddEditEventInputModel
    {
        public long? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long CourtId { get; set; }
        public bool RequireApproval { get; set; }
    }
}
