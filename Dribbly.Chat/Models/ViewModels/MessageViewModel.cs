using Dribbly.Chat.Enums;
using Dribbly.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dribbly.Chat.Models.ViewModels
{
    public class MessageViewModel
    {
        public long MessageId { get; set; }
        public MessageRecipientStatusEnum Status { get; set; }
        public string Text { get; set; }
        public long SenderId { get; set; }
        public bool IsSender { get; set; }
        public long? ChatId { get; set; }
        public DateTime DateSent { get; set; }
        public List<MultimediaModel> MediaCollection { get; set; }

        public MessageViewModel(MessageModel message, long forParticipantId)
        {
            var participant = message.Participants.SingleOrDefault(p => p.ParticipantId == forParticipantId);
            if (participant != null)
            {
                MessageId = message.Id;
                Text = message.Text;
                SenderId = message.SenderId;
                IsSender = participant.IsSender;
                Status = participant.Status;
                ChatId = message.ChatId;
                DateSent = message.DateAdded;
                MediaCollection = message.MediaCollection.Select(m => m.Media).ToList();
            }
        }
    }
}
