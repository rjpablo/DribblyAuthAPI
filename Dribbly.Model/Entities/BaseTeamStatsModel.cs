using Dribbly.Core.Models;
using Dribbly.Model.Teams;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    public abstract class BaseTeamStatsModel : BaseEntityModel
    {
        [ForeignKey(nameof(Team))]
        public long TeamId { get; set; }

        public TeamModel Team { get; set; }
        /// <summary>
        /// Games played
        /// </summary>
        public int GP { get; set; }
        /// <summary>
        /// Games Won
        /// </summary>
        public int GW { get; set; }
        /// <summary>
        /// Points per game
        /// </summary>
        public double PPG { get; set; }
        /// <summary>
        /// Rebounds per game
        /// </summary>
        public double RPG { get; set; }
        /// <summary>
        /// Assists per game
        /// </summary>
        public double APG { get; set; }
        /// <summary>
        /// Field Goal Percentage
        /// </summary>
        public double FGP { get; set; }
        /// <summary>
        /// Blocks per game
        /// </summary>
        public double BPG { get; set; }
        /// <summary>
        /// 3-point percentage
        /// </summary>
        public double ThreePP { get; set; }
        public double OverallScore { get; set; }
    }
}
