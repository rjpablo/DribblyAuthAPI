using Dribbly.Core.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Chat.Models
{
    [Table("MessageMedia")]
    public class MessageMediaModel : BaseEntityModel
    {
        [ForeignKey(nameof(Message))]
        public long MessageId { get; set; }
        [ForeignKey(nameof(Media))]
        public long MediaId { get; set; }
        [JsonIgnore]
        public virtual MessageModel Message { get; set; }
        public virtual MultimediaModel Media { get; set; }
    }
}
