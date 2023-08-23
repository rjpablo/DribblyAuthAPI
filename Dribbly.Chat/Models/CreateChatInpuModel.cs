using Dribbly.Chat.Enums;
using System.Collections.Generic;

namespace Dribbly.Chat.Models
{
    public class CreateChatInpuModel
    {
        public ICollection<MessageModel> Messages { get; set; }
        public string Title { get; set; }
        public ICollection<ChatParticipantModel> Participants { get; set; }
        public ChatTypeEnum Type { get; set; }
    }
}
