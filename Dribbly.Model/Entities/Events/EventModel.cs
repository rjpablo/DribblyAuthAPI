using Dribbly.Core.Enums;
using Dribbly.Core.Models;
using Dribbly.Model.Courts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities.Events
{
    [Table("Events")]
    public class EventModel:BaseEntityModel, IIndexedEntity
    {
        public string Title { get; set; }

        [NotMapped]
        public string Name => Title;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public EntityTypeEnum EntityType => EntityTypeEnum.Event;

        [ForeignKey(nameof(Court))]
        public long? CourtId { get; set; }

        [ForeignKey(nameof(Logo))]
        public long? LogoId { get; set; }

        public string IconUrl => Logo?.Url;

        public MultimediaModel Logo { get; set; }

        public EntityStatusEnum EntityStatus { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Participants need to be approved
        /// </summary>
        public bool RequireApproval { get; set; } = true;

        [ForeignKey(nameof(AddedBy))]
        public long AddedById { get; set; }
        public AccountModel AddedBy { get; set; }

        public ICollection<EventAttendeeModel> Attendees { get; set; }
        public CourtModel Court { get; set; }
    }
}
