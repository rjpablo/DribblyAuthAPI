using Dribbly.Core.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Chat.Models
{
    [Table("ChatParticipants")]
    public class ChatParticipantModel : BaseEntityModel
    {
        public string Name { get; set; }
        [ForeignKey("Chat")]
        public long ChatId { get; set; }
        //[ForeignKey("Participant")]
        public long ParticipantId { get; set; }
        [JsonIgnore]
        public virtual ChatModel Chat { get; set; }
        //public virtual IdentityUser Participant { get; set; }
        [ForeignKey(nameof(Photo))]
        public long? PhotoId { get; set; }
        public PhotoModel Photo { get; set; }
    }
}
