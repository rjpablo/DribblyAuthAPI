using Dribbly.Chat.Enums;
using System.Collections.Generic;

namespace Dribbly.Chat.Models
{
    public class CreateChatInpuModel
    {
        public ICollection<MessageModel> Messages { get; set; } = new List<MessageModel>();
        public string Title { get; set; }
        public List<long> ParticipantIds { get; set; } = new List<long>();
        public ChatTypeEnum Type { get; set; }
        public string Code { get; set; }
        public long? IconId { get; set; }
    }
}
