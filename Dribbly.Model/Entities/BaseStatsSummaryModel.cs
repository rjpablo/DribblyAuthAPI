using Dribbly.Core.Extensions;
using Dribbly.Core.Models;

namespace Dribbly.Model.Entities
{
    public class BaseStatsSummaryModel : BaseEntityModel
    {
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
        /// Turnovers per game
        /// </summary>
        public int TPG { get; set; }
        /// <summary>
        /// 3-point percentage
        /// </summary>

        /// <summary>
        /// Steals per game
        /// </summary>
        public int SPG { get; set; }
        public double ThreePP { get; set; }
        public double OverallScore { get; set; }

        public void SetOverallScore()
        {
            OverallScore = (GW.DivideBy(GP)) + (PPG / 118) + (APG / 28.2) + (BPG / 6.4) + (RPG / 47.7);
        }
    }
}
