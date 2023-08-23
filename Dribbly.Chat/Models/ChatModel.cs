using Dribbly.Chat.Enums;
using Dribbly.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Chat.Models
{
    [Table("Chats")]
    public class ChatModel : BaseEntityModel
    {
        public ICollection<MessageModel> Messages { get; set; } = new List<MessageModel>();
        public DateTime LastUpdateTime { get; set; }
        public string Title { get; set; }
        public ICollection<ChatParticipantModel> Participants { get; set; }
        public ChatTypeEnum Type { get; set; }
        /// <summary>
        /// TRUE for chats with no messages yet
        /// </summary>
        public bool IsTemporary { get; set; }
        [ForeignKey(nameof(Icon))]
        public long? IconId { get; set; }
        public MultimediaModel Icon { get; set; }
    }
}
