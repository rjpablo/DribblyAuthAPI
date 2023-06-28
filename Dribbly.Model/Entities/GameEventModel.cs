using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Enums;
using Dribbly.Model.Games;
using Dribbly.Model.Teams;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    [Table("GameEvents")]
    public class GameEventModel: BaseEntityModel
    {
        [ForeignKey(nameof(Game))]
        public long GameId { get; set; }
        [ForeignKey(nameof(Team))]
        public long TeamId { get; set; }
        [ForeignKey(nameof(PerformedBy))]
        public long PerformedById { get; set; }
        public GameEventTypeEnum Type { get; set; }
        public string AdditionalData { get; set; }
        public TeamModel Team { get; set; }
        [JsonIgnore]
        public GameModel Game { get; set; }
        public AccountModel PerformedBy { get; set; }
    }
}
