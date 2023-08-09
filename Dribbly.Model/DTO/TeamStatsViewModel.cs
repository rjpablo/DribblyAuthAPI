using Dribbly.Model.Courts;
using Dribbly.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.DTO
{
    public class TeamStatsViewModel
    {
        #region Team Info
        public PhotoModel Logo { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        #endregion

        #region Stat info
        public long TeamId { get; set; }
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
        #endregion

        public TeamStatsViewModel(BaseTeamStatsModel source)
        {
            // Team Info
            Logo = source.Team.Logo;
            Name = source.Team.Name;
            ShortName = source.Team.ShortName;

            // Stat info
            TeamId = source.TeamId;
            GP = source.GP;
            GW = source.GW;
            PPG = source.PPG;
            RPG = source.RPG;
            APG = source.APG;
            FGP = source.FGP;
            BPG = source.BPG;
            ThreePP = source.ThreePP;
            OverallScore = source.OverallScore;
        }
    }
}
