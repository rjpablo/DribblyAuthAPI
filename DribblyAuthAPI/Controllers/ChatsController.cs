using Dribbly.Chat.Models;
using Dribbly.Chat.Services;
using Dribbly.Core.Utilities;
using Dribbly.Service.Services;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Chats")]
    [Authorize]
    public class ChatsController : BaseController
    {
        private readonly IDribblyChatService _chatsService;
        private readonly ISecurityUtility _securityUtility;

        public ChatsController(IDribblyChatService chatsService,
            ISecurityUtility securityUtility)
        {
            _chatsService = chatsService;
            _securityUtility = securityUtility;
        }

        [HttpGet]
        [Route("GetChats")]
        public async Task<IHttpActionResult> GetChats()
        {
            var chats = await _chatsService.GetChatsAsync(GetCurrentAccountId());
            return Ok(chats);
        }

        [HttpGet]
        [Route("GetChat/{chatId}")]
        public async Task<IHttpActionResult> GetChat(long chatId)
        {
            var chats = await _chatsService.GetChatAsync(chatId, GetCurrentAccountId());
            return Ok(chats);
        }

        [HttpGet]
        [Route("GetPrivateChat/{withUserId}")]
        public async Task<IHttpActionResult> GetPrivateChat(long withUserId)
        {
            var chat = await _chatsService.GetPrivateChatAsync(withUserId, GetCurrentAccountId());
            return Ok(chat);
        }

        [HttpPost]
        [Route("SendMessage")]
        public async Task<IHttpActionResult> SendMessage(MessageModel message)
        {
            var chat = await _chatsService.SendMessage(message, GetCurrentAccountId());
            return Ok(chat);
        }

        [HttpPost]
        [Route("MarkMessageAsSeen/{chatId}/{messageId}")]
        public async Task<IHttpActionResult> MarkMessageAsSeen(long chatId, long messageId)
        {
            return Ok(await _chatsService.MarkMessageAsSeenAsync(chatId, messageId, GetCurrentAccountId()));
        }

        [HttpGet]
        [Route("GetUnviewedCount/{chatId}")]
        public async Task<IHttpActionResult> GetUnviewedCount(long chatId)
        {
            return Ok(await _chatsService.GetUnviewedCountAsync(chatId, GetCurrentAccountId()));
        }

        [HttpPost]
        [Route("CreateChat")]
        public async Task<IHttpActionResult> CreateChat(CreateChatInpuModel chat)
        {
            try
            {
                var result = await _chatsService.CreateChatAsync(chat, GetCurrentAccountId());
                return Ok(result);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("GetOrCreatePrivateChat/{withUserId}")]
        public async Task<IHttpActionResult> GetOrCreatePrivateChat(long withUserId, [FromBody] CreateChatInpuModel chat)
        {
            try
            {
                var result = await _chatsService.GetOrCreatePrivateChatAsync(withUserId, chat, GetCurrentAccountId());
                return Ok(result);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private long GetCurrentAccountId()
        {
            return _securityUtility.GetAccountId().Value;
        }
    }
}
