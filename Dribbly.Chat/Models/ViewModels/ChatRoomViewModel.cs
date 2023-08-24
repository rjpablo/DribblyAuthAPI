using Dribbly.Chat.Enums;
using Dribbly.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dribbly.Chat.Models.ViewModels
{
    public class ChatRoomViewModel
    {
        public long ChatId { get; set; }
        public string RoomName { get; set; }
        public ChatTypeEnum Type { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public ICollection<MessageViewModel> Messages { get; set; } = new List<MessageViewModel>();
        /// <summary>
        /// The number of unviewed updates
        /// </summary>
        public int UnviewedCount { get; set; }
        public ICollection<ChatParticipantModel> Participants { get; set; }
        public MultimediaModel RoomIcon { get; set; }

        public ChatRoomViewModel(ChatModel chat, long forParticipantId)
        {
            ChatId = chat.Id;
            RoomName = chat.Title;
            Type = chat.Type;
            LastUpdateTime = chat.LastUpdateTime;
            foreach (var message in chat.Messages)
            {
                Messages.Add(new MessageViewModel(message, forParticipantId));
            }
            UnviewedCount = Messages.Count(m => !m.IsSender && m.Status == MessageRecipientStatusEnum.NotSeen);
            Participants = chat.Participants;
            if (Type == ChatTypeEnum.Private)
            {
                RoomName = chat.Participants.Single(p => p.ParticipantId != forParticipantId).Name;
                //RoomIcon = chat.Participants.Single(p => p.ParticipantId != forParticipantId).Photo;
            }
        }
    }
}
