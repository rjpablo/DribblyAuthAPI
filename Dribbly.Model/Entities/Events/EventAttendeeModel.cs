using Dribbly.Core.Models;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities.Events
{
    [Table("EventAttendees")]
    public class EventAttendeeModel
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey(nameof(Account))]
        public long AccountId { get; set; }
        [ForeignKey(nameof(Event))]
        public long EventId { get; set; }
        public DateTime DateJoined { get; set; }
        public bool IsApproved { get; set; }
        public bool IsAdmin { get; set; }
        [NotMapped]
        public string Name => Account?.Name;
        [NotMapped]
        public string IconUrl => Account?.ProfilePhoto?.Url;
        public AccountModel Account { get; set; }
        [JsonIgnore]
        public EventModel Event { get; set; }
    }
}
