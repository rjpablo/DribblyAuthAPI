using Dribbly.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Chat.Models
{
    [Table("Messages")]
    public class MessageModel : BaseEntityModel
    {
        public long SenderId { get; set; }
        public string Text { get; set; }
        [ForeignKey(nameof(Chat))]
        public long? ChatId { get; set; }
        [JsonIgnore]
        public virtual ChatModel Chat { get; set; }
        public virtual ICollection<ParticipantMessageModel> Participants { get; set; } = new List<ParticipantMessageModel>();
        public virtual ICollection<MessageMediaModel> MediaCollection { get; set; } = new List<MessageMediaModel>();
        public MessageModel() { }

        public MessageModel(long senderId, string text, long chatId)
        {
            SenderId = senderId;
            Text = text;
            ChatId = chatId;
            DateAdded = DateTime.UtcNow;
        }
    }
}
