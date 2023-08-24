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
            var chatIds = (await _context.ChatParticipants.Where(p => p.ParticipantId == userId).ToListAsync())
                .Select(p => p.ChatId);
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
            var chatIds = (await _context.ChatParticipants.Where(p => p.ParticipantId == userId).ToListAsync())
                .Select(p => p.ChatId);
            // we don't filter by IsTemporary here so temporary chat rooms can be opened and messages can be sent
            var chats = await _context.Chats
                .Include(c => c.Participants.Select(p => p.Photo))
                .Include(c => c.Messages.Select(m => m.Participants))
                .Where(c => chatIds.Contains(c.Id) && c.Type == Enums.ChatTypeEnum.Private)
                .OrderByDescending(c => c.LastUpdateTime)
                .ToListAsync();
            return chats.Where(c => c.Participants.Select(p => p.ParticipantId).Contains(withUserId)).SingleOrDefault();
        }

        public async Task UpdateParticipantPhoto(long participantUserId, MultimediaModel photo)
        {
            var participant = await _context.ChatParticipants.Where(p => p.ParticipantId == participantUserId)
                .FirstOrDefaultAsync();
            participant.PhotoId = photo.Id;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ChatRoomViewModel>> GetChatsAsync(long userId)
        {
            var chatIds = (await _context.ChatParticipants.Where(p => p.ParticipantId == userId).ToListAsync())
                .Select(p => p.ChatId);
            var chats = await _context.Chats
                .Include(c => c.Icon)
                .Include(c => c.Messages.Select(m => m.Participants))
                .Include(c => c.Messages.Select(m => m.MediaCollection.Select(cl => cl.Media)))
                .Include(c => c.Participants.Select(p => p.Photo))
                .Where(c => chatIds.Contains(c.Id) && !c.IsTemporary && c.Type == Enums.ChatTypeEnum.Private)
                .OrderByDescending(c => c.LastUpdateTime)
                .ToListAsync();
            return chats.Select(c => new ChatRoomViewModel(c, userId));
        }

        public async Task<ChatModel> GetChatMessagesAsync(long withUserId, long userId)
        {
            var chatIds = (await _context.ChatParticipants.Where(p => p.ParticipantId == userId).ToListAsync())
                .Select(p => p.ChatId);
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
                .Include(c => c.Participants.Select(p => p.Photo))
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
            var participants = _context.ChatParticipants.Where(p => p.ChatId == message.ChatId).ToList();

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
            foreach (var message in input.Messages)
            {
                message.SenderId = senderId;
                message.DateAdded = DateTime.UtcNow;
                chat.Messages.Add(message);
            }
            foreach (var p in input.Participants)
            {
                if (p.Photo != null)
                {
                    p.Photo.DateAdded = DateTime.UtcNow;
                    p.Photo.Type = Core.Enums.MultimediaTypeEnum.Photo;
                }
                p.DateAdded = DateTime.UtcNow;
            }
            chat.Participants = input.Participants;
            chat.DateAdded = DateTime.UtcNow;
            chat.LastUpdateTime = DateTime.UtcNow;
            chat.Type = Enums.ChatTypeEnum.Private;
            chat.IsTemporary = chat.Messages.Count == 0;
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            foreach (var message in input.Messages)
            {
                foreach (var p in chat.Participants)
                {
                    _context.ParticipantMessages
                        .Add(new ParticipantMessageModel(message.Id, p.ParticipantId, p.ParticipantId == senderId));
                }
            }

            await _context.SaveChangesAsync();


            foreach (var message in chat.Messages)
            {
                foreach (var p in chat.Participants)
                {
                    _chat.ReceiveMessage(p.ParticipantId.ToString(), new MessageViewModel(message, p.ParticipantId));
                }
            }

            return chat;
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
        Task UpdateParticipantPhoto(long participantUserId, MultimediaModel photo);
    }
}
