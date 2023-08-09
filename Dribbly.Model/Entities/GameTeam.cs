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
        public int Score { get; set; }
        public int TeamFoulCount { get; set; }
        /// <summary>
        /// The total number of timeouts left
        /// </summary>
        public int TimeoutsLeft { get; set; }
        public int FullTimeoutsUsed { get; set; }
        public int ShortTimeoutsUsed { get; set; }
        public bool IsInBonus { get; set; }
        public bool? Won { get; set; }

        #region Stats
        /// <summary>
        /// field goal attempts
        /// </summary>
        public int FGA { get; set; }
        /// <summary>
        /// field goals made
        /// </summary>
        public int FGM { get; set; }
        /// <summary>
        /// 3pt attempts
        /// </summary>
        public int ThreePA { get; set; }
        /// <summary>
        /// 3pts made
        /// </summary>
        public int ThreePM { get; set; }
        public int Blocks { get; set; }
        public int Rebounds { get; set; }
        public int Assists { get; set; }
        public int Turnovers { get; set; }
        public int Steals { get; set; }
        #endregion Stats
        #endregion Mapped Columns

        #region Unmapped Columns
        [NotMapped]
        public string Name { get => Team?.Name; }
        [NotMapped]
        public string ShortName { get => Team?.ShortName; }
        #endregion

        public ICollection<GamePlayerModel> Players { get; set; } = new List<GamePlayerModel>();
    }
}
