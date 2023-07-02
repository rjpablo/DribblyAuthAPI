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
        #region Mapped Columns
        [ForeignKey(nameof(Game))]
        public long GameId { get; set; }
        [ForeignKey(nameof(Team))]
        public long TeamId { get; set; }
        public TeamModel Team { get; set; }
        [JsonIgnore]
        public GameModel Game { get; set; }
        public int TeamFoulCount { get; set; }
        /// <summary>
        /// The total number of timeouts left
        /// </summary>
        public int TimeoutsLeft { get; set; }
        public int FullTimeoutsUsed { get; set; }
        public int ShortTimeoutsUsed { get; set; }
        #endregion

        #region Unmapped Columns
        [NotMapped]
        public string Name { get => Team?.Name; }
        #endregion

        public ICollection<GamePlayerModel> Players { get; set; } = new List<GamePlayerModel>();
    }
}
