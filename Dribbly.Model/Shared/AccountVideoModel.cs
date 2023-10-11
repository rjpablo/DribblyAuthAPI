using Dribbly.Model.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Shared
{
    [Table("AccountVideos")]
    public class AccountVideoModel
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Video")]
        public long VideoId { get; set; }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("Account")]
        public long AccountId { get; set; }

        public virtual PlayerModel Account { get; set; }
        public virtual VideoModel Video { get; set; }
    }
}
