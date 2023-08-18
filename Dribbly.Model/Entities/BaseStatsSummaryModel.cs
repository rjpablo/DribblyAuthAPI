using Dribbly.Core.Extensions;
using Dribbly.Core.Models;
using System.Collections.Generic;
using System.Linq;

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
        public double? FGP { get; set; }
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
        public double? ThreePP { get; set; }
        /// <summary>
        /// Free Throw Percentage
        /// </summary>
        public double? FTP { get; set; }

        /// <summary>
        /// Steals per game
        /// </summary>
        public int SPG { get; set; }
        public double OverallScore { get; set; }

        public void UpdateStats(IEnumerable<BaseStatsModel> allGameStats)
        {
            GP = allGameStats.Count();
            GW = allGameStats.Count(s => s.Won.Value);
            PPG = allGameStats.Average(s => s.Points);
            RPG = allGameStats.Average(s => s.Rebounds);
            APG = allGameStats.Average(s => s.Assists);
            BPG = allGameStats.Average(s => s.Blocks);
            FGP = allGameStats.Average(s => s.FGM.DivideBy(s.FGA));
            ThreePP = allGameStats.Average(s => s.ThreePM.DivideBy(s.ThreePA));
            FTP = allGameStats.Average(s => s.FTM.DivideBy(s.FTA));
        }

        public void SetOverallScore()
        {
            OverallScore = (GW.DivideBy(GP) ?? 0) + (PPG / 118) + (APG / 28.2) + (BPG / 6.4) + (RPG / 47.7) + (FTP ?? 0);
        }
    }
}
