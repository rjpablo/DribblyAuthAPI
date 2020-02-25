﻿using DribblyAuthAPI.Models.Courts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DribblyAuthAPI.Models.Shared
{
    /// <summary>
    /// All games (any sport) should extend this model
    /// </summary>
    [Table("Events")]
    public abstract class EventModel: BaseModel
    {
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        public string Title { get; set; }
        public string AddedBy { get; set; }
        [ForeignKey("Court"), Required]
        public long CourtId { get; set; }

        // navigation properties
        public virtual CourtModel Court { get; set; }
    }
}