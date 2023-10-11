using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    [Table("AccountHighlights")]
    public class AccountHighlightModel
    {
        [Key, Column(Order = 1)]
        [ForeignKey(nameof(Account))]
        public long AccountId { get; set; }
        [Key, Column(Order = 2)]
        [ForeignKey(nameof(File))]
        public long FileId { get; set; }
        public MultimediaModel File { get; set; }
        [JsonIgnore]
        public PlayerModel Account { get; set; }
    }
}
