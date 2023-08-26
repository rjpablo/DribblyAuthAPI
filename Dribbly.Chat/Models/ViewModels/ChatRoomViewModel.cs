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
        public string Code { get; set; }
        public ChatTypeEnum Type { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public ICollection<MessageViewModel> Messages { get; set; } = new List<MessageViewModel>();
        /// <summary>
        /// The number of unviewed updates
        /// </summary>
        public int UnviewedCount { get; set; }
        public ICollection<AccountModel> Participants { get; set; }
        public MultimediaModel RoomIcon { get; set; }

        public ChatRoomViewModel(ChatModel chat, long forParticipantId)
        {
            ChatId = chat.Id;
            RoomName = chat.Title;
            Code = chat.Code;
            Type = chat.Type;
            LastUpdateTime = chat.LastUpdateTime;
            foreach (var message in chat.Messages)
            {
                Messages.Add(new MessageViewModel(message, forParticipantId));
                UnviewedCount += message.Participants.Count(p => p.ParticipantId == forParticipantId
                && message.SenderId != p.ParticipantId
                && p.Status == MessageRecipientStatusEnum.NotSeen);
            }
            Participants = chat.Participants.Select(p => p.Participant).ToList();
            if(Type == ChatTypeEnum.Team)
            {
                RoomIcon = chat.Icon;
            }
        }
    }
}
