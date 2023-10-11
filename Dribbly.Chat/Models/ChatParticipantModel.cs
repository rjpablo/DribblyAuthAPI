using Dribbly.Core.Models;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Chat.Models
{
    [Table("ChatParticipants")]
    public class ChatParticipantModel
    {
        [Key, Column(Order = 1)]
        [ForeignKey(nameof(Chat))]
        public long ChatId { get; set; }
        [Key, Column(Order = 2)]
        [ForeignKey(nameof(Participant))]
        public long ParticipantId { get; set; }
        [JsonIgnore]
        public virtual ChatModel Chat { get; set; }
        public AccountModel Participant { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
