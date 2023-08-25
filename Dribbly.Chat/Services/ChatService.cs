using Dribbly.Chat.Hubs;
using Dribbly.Chat.Models;
using Dribbly.Chat.Models.ViewModels;
using Dribbly.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using Dribbly.Chat.Data;
using Microsoft.AspNet.SignalR;

namespace Dribbly.Chat.Services
{
    public class DribblyChatService : IDribblyChatService
    {
        private readonly IChatDbContext _context;
        private readonly IChatHub _chat;
        private static IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();


        public DribblyChatService(IChatDbContext context)
        {
            _context = context;
            _chat = new ChatHub();
        }

        public async Task<IEnumerable<ChatModel>> GetChatsAsync_backup(long userId)
        {
            //var chatIds = (await _context.ChatParticipants.Where(p => p.ParticipantId == userId).ToListAsync())
            //    .Select(p => p.ChatId);
            var chatIds = new List<long>();
            return await _context.Chats.Include(c => c.Participants).Where(c => chatIds.Contains(c.Id))
                .OrderByDescending(c => c.LastUpdateTime)
                .ToListAsync();
        }

        public async Task<ChatRoomViewModel> GetOrCreatePrivateChatAsync(long withUserId, CreateChatInpuModel input, long userId)
        {
            var chat = await GetPrivateChatAsync(withUserId, userId);
            if (chat == null)
            {
                chat = await CreateChatAsync(input, userId);
            }
            return new ChatRoomViewModel(chat, userId);
        }

        public async Task<ChatModel> GetPrivateChatAsync(long withUserId, long userId)
        {
            var code = GetPrivateChatCode(new long[] { withUserId, userId });
            // we don't filter by IsTemporary here so temporary chat rooms can be opened and messages can be sent
            var chat = await _context.Chats
                .Include(c => c.Participants.Select(p => p.Participant.ProfilePhoto))
                .Include(c => c.Messages.Select(m => m.Participants))
                .Where(c => c.Code == code)
                .OrderByDescending(c => c.LastUpdateTime)
                .SingleOrDefaultAsync();
            return chat;
        }

        public async Task<IEnumerable<ChatRoomViewModel>> GetChatsAsync(long userId)
        {
            var chatIds = (await _context.ChatParticipants.Where(p => p.ParticipantId == userId).ToListAsync())
                .Select(p => p.ChatId);
            var chats = await _context.Chats
                .Include(c => c.Icon)
                .Include(c => c.Messages.Select(m => m.Participants))
                .Include(c => c.Messages.Select(m => m.MediaCollection.Select(cl => cl.Media)))
                .Include(c => c.Participants.Select(p => p.Participant.ProfilePhoto))
                .Where(c => chatIds.Contains(c.Id) && !c.IsTemporary && c.Type == Enums.ChatTypeEnum.Private)
                .OrderByDescending(c => c.LastUpdateTime)
                .ToListAsync();
            return chats.Select(c => new ChatRoomViewModel(c, userId));
        }

        public async Task<ChatModel> GetChatMessagesAsync(long withUserId, long userId)
        {
            //var chatIds = (await _context.ChatParticipants.Where(p => p.ParticipantId == userId).ToListAsync())
            //    .Select(p => p.ChatId);
            var chatIds = new List<long>();
            var chats = await _context.Chats
                .Include(c => c.Messages.Select(m => m.Participants))
                .Where(c => chatIds.Contains(c.Id) && c.Type == Enums.ChatTypeEnum.Private)
                .OrderByDescending(c => c.LastUpdateTime)
                .ToListAsync();
            return chats.Where(c => c.Participants.Select(p => p.ParticipantId).Contains(withUserId)).SingleOrDefault();
        }

        public async Task<ChatRoomViewModel> GetChatAsync(long chatId, long userId)
        {
            // we don't filter by IsTemporary here. This is used to open a chat room when client receives a message
            var chat = await _context.Chats
                .Include(c => c.Icon)
                .Include(c => c.Messages.Select(m => m.Participants))
                .Include(c => c.Messages.Select(m => m.MediaCollection.Select(cl => cl.Media)))
                .Include(c => c.Participants.Select(p => p.Participant.ProfilePhoto))
                .Where(c => c.Id == chatId && c.Type == Enums.ChatTypeEnum.Private)
                .OrderByDescending(c => c.LastUpdateTime)
                .SingleOrDefaultAsync();
            return chat != null ? new ChatRoomViewModel(chat, userId) : null;
        }

        public async Task<int> MarkMessageAsSeenAsync(long chatId, long messageId, long userId)
        {
            var message = _context.ParticipantMessages
                .Where(m => m.MessageId == messageId && m.ParticipantId == userId)
                .SingleOrDefault();
            if (message != null)
            {
                message.Status = Enums.MessageRecipientStatusEnum.Seen;
            }
            await _context.SaveChangesAsync();
            int unviewedCount = await GetUnviewedCountAsync(chatId, userId);

            _hubContext.Clients.Group(userId.ToString())
                .unviewedCountChanged(new { chatId = chatId, unviewedCount = unviewedCount });
            return unviewedCount;
        }

        public async Task<int> GetUnviewedCountAsync(long chatId, long userId)
        {
            var chats = await _context.Chats
                .Include(m => m.Messages.Select(msg => msg.Participants))
                .Where(c => c.Id == chatId)
                .SingleOrDefaultAsync();
            int count = 0;
            foreach (var message in chats.Messages)
            {
                count = count + message.Participants.Count(p => p.ParticipantId == userId && !p.IsSender && p.Status == Enums.MessageRecipientStatusEnum.NotSeen);
            }
            return count;
        }

        public async Task<MessageModel> SendMessage(MessageModel message, long senderId)
        {
            var chat = _context.Chats
                .Include(m => m.Messages)
                .Include(m => m.Participants)
                .Where(c => c.Id == message.ChatId).First();
            //var participants = _context.ChatParticipants.Where(p => p.ChatId == message.ChatId).ToList();

            try
            {
                message.DateAdded = DateTime.UtcNow;
                message.SenderId = senderId;
                foreach (var p in chat.Participants)
                {
                    message.Participants.Add(new ParticipantMessageModel
                    {
                        ParticipantId = p.ParticipantId,
                        IsSender = p.ParticipantId == senderId,
                        DateAdded = DateTime.UtcNow,
                        Status = p.ParticipantId == senderId ?
                          Enums.MessageRecipientStatusEnum.Seen :
                          Enums.MessageRecipientStatusEnum.NotSeen
                    });
                }
                _context.Messages.Add(message);
                chat.IsTemporary = false;
                chat.LastUpdateTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                //TODO: log error
                throw;
            }

            // load multimedia before sending to clients
            foreach (var media in message.MediaCollection)
            {
                var messageMedia = _context.MessageMedia
                    .Include(m => m.Media)
                    .Where(m => m.MediaId == media.MediaId && m.MessageId == media.MessageId)
                    .Single();
                media.Media = messageMedia.Media;
            }

            foreach (var p in chat.Participants)
            {
                var group = _hubContext.Clients.Group(p.ParticipantId.ToString());
                if (group != null)
                    group.ReceiveMessage(new MessageViewModel(message, p.ParticipantId));
            }

            return message;
        }

        /// <summary>
        /// Used for creating PRIVATE chats
        /// </summary>
        /// <param name="input"></param>
        /// <param name="senderId"></param>
        /// <returns></returns>
        public async Task<ChatModel> CreateChatAsync(CreateChatInpuModel input, long senderId)
        {
            ChatModel chat = new ChatModel();
            chat.Title = input.Title;
            var participants = await _context.Accounts.Where(a => input.ParticipantIds.Contains(a.Id)).ToListAsync();
            foreach (var message in input.Messages)
            {
                message.SenderId = senderId;
                message.DateAdded = DateTime.UtcNow;
                chat.Messages.Add(message);
            }
            chat.Participants = input.ParticipantIds.Select(p => new ChatParticipantModel
            {
                DateAdded = DateTime.UtcNow,
                ParticipantId = p
            }).ToList();
            chat.DateAdded = DateTime.UtcNow;
            chat.LastUpdateTime = DateTime.UtcNow;
            chat.Type = Enums.ChatTypeEnum.Private;
            chat.IsTemporary = chat.Messages.Count == 0;
            chat.Code = GetPrivateChatCode(input.ParticipantIds);
            _context.Chats.Add(chat);

            foreach (var message in chat.Messages)
            {
                foreach (var p in chat.Participants)
                {
                    message.Participants
                         .Add(new ParticipantMessageModel
                         {
                             ParticipantId = p.ParticipantId,
                             IsSender = p.ParticipantId == message.SenderId,
                             Status = p.ParticipantId == message.SenderId ?
                                 Enums.MessageRecipientStatusEnum.Seen :
                                 Enums.MessageRecipientStatusEnum.NotSeen,
                             DateAdded = DateTime.UtcNow
                         });
                }
            }

            await _context.SaveChangesAsync();

            chat = await _context.Chats
                .Include(c => c.Participants.Select(p => p.Participant.ProfilePhoto))
                .Include(c => c.Messages.Select(m => m.Participants.Select(p => p.Participant.ProfilePhoto)))
                .SingleOrDefaultAsync();

            foreach (var message in chat.Messages)
            {
                foreach (var p in chat.Participants)
                {
                    _chat.ReceiveMessage(p.ParticipantId.ToString(), new MessageViewModel(message, p.ParticipantId));
                }
            }

            return chat;
        }

        private string GetPrivateChatCode(IEnumerable<long> participantIds)
        {
            return "pr" + participantIds.Min() + "-" + participantIds.Max();
        }
    }

    public interface IDribblyChatService
    {
        Task<IEnumerable<ChatRoomViewModel>> GetChatsAsync(long userId);
        Task<ChatRoomViewModel> GetChatAsync(long chatId, long userId);
        Task<MessageModel> SendMessage(MessageModel message, long senderId);
        Task<ChatModel> CreateChatAsync(CreateChatInpuModel chat, long senderId);
        Task<ChatModel> GetPrivateChatAsync(long withUserId, long userId);
        Task<ChatRoomViewModel> GetOrCreatePrivateChatAsync(long withUserId, CreateChatInpuModel input, long userId);
        Task<int> MarkMessageAsSeenAsync(long chatId, long messageId, long userId);
        Task<int> GetUnviewedCountAsync(long chatId, long userId);
    }
}
