using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Courts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Shared
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

        public string BookedById { get; set; }

        //public string BookedFor { get; set; }

        // navigation properties
        public virtual CourtModel Court { get; set; }

        public virtual AccountBasicInfoModel BookedBy { get; set; }

        public virtual AccountsChoicesItemModel BookedByChoice { get; set; }
    }
}