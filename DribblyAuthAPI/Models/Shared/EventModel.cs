using DribblyAuthAPI.Models.Courts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DribblyAuthAPI.Models.Shared
{
    /// <summary>
    /// All games (any sport) should extend this model
    /// </summary>
    [Table("Events")]
    public abstract class EventModel: BaseEntityModel
    {
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        [MinLength(5)]
        public string Title { get; set; }
        [Required]
        public string AddedBy { get; set; }
        [ForeignKey("Court"), Required]
        public long CourtId { get; set; }

        // navigation properties
        public virtual CourtModel Court { get; set; }
    }
}