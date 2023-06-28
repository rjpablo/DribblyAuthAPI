using Dribbly.Core.Models;
using Dribbly.Model.Games;
using Dribbly.Model.Teams;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    [Table("GameTeams")]
    public class GameTeamModel : BaseEntityModel
    {
        [ForeignKey(nameof(Game))]
        public long GameId { get; set; }
        [ForeignKey(nameof(Team))]
        public long TeamId { get; set; }
        public TeamModel Team { get; set; }
        [JsonIgnore]
        public GameModel Game { get; set; }
        public int TeamFoulCount { get; set; }
        public ICollection<GamePlayerModel> Players { get; set; } = new List<GamePlayerModel>();
    }
}
