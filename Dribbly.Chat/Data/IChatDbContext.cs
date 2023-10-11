using Dribbly.Chat.Models;
using Dribbly.Core.Models;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Dribbly.Chat.Data
{
    public interface IChatDbContext
    {
        #region Chat
        DbSet<ChatModel> Chats { get; set; }
        DbSet<ChatParticipantModel> ChatParticipants { get; set; }
        DbSet<ParticipantMessageModel> ParticipantMessages { get; set; }
        DbSet<MessageModel> Messages { get; set; }
        DbSet<MessageMediaModel> MessageMedia { get; set; }
        DbSet<MultimediaModel> Multimedia { get; set; }
        DbSet<AccountModel> Accounts { get; set; }
        Database Database { get; }
        Task<int> SaveChangesAsync();
        #endregion
    }
}
