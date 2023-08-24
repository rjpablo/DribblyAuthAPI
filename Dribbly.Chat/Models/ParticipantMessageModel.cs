using Dribbly.Chat.Enums;
using Dribbly.Core.Models;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Chat.Models
{
    /// <summary>
    /// Represents a recipient/sender of a message. A message always has only one sender but it may have
    /// multiple recipients (e.g. group chat)
    /// </summary>
    [Table("ParticipantMessages")]
    public class ParticipantMessageModel : BaseEntityModel
    {
        [ForeignKey(nameof(Message))]
        public long MessageId { get; set; }
        public long ParticipantId { get; set; }
        /// <summary>
        /// Whether or not the participant is the sender of the message
        /// </summary>
        public bool IsSender { get; set; }
        public MessageRecipientStatusEnum Status { get; set; } = MessageRecipientStatusEnum.NotSeen;
        [JsonIgnore]
        public virtual MessageModel Message { get; set; }
        public ParticipantMessageModel() { }

        public ParticipantMessageModel(long messageId, long participantId, bool isSender, MessageRecipientStatusEnum status = MessageRecipientStatusEnum.NotSeen)
        {
            MessageId = messageId;
            ParticipantId = participantId;
            IsSender = isSender;
            Status = isSender ? MessageRecipientStatusEnum.Seen : status;
            DateAdded = DateTime.UtcNow;
        }

    }
}
