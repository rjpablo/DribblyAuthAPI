using Dribbly.Chat.Enums;
using Dribbly.Core.Models;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Chat.Models
{
    /// <summary>
    /// Represents a recipient/sender of a message. A message always has only one sender but it may have
    /// multiple recipients (e.g. group chat)
    /// </summary>
    [Table("ParticipantMessages")]
    public class ParticipantMessageModel
    {
        [Key, Column(Order = 1)]
        [ForeignKey(nameof(Message))]
        public long MessageId { get; set; }
        [Key, Column(Order = 2)]
        [ForeignKey(nameof(Participant))]
        public long ParticipantId { get; set; }
        /// <summary>
        /// Whether or not the participant is the sender of the message
        /// </summary>
        public bool IsSender { get; set; }
        public MessageRecipientStatusEnum Status { get; set; } = MessageRecipientStatusEnum.NotSeen;
        [JsonIgnore]
        public virtual MessageModel Message { get; set; }
        public AccountModel Participant { get; set; }
        public DateTime DateAdded { get; set; }
        public ParticipantMessageModel() { }

        public ParticipantMessageModel(MessageModel message, long participantId)
        {
            MessageId = message.Id;
            ParticipantId = participantId;
            IsSender = message.SenderId == participantId;
            Status = message.SenderId == participantId ? MessageRecipientStatusEnum.Seen : MessageRecipientStatusEnum.NotSeen;
            DateAdded = DateTime.UtcNow;
        }

    }
}
