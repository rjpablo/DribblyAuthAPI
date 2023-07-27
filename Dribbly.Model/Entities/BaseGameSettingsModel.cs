using Dribbly.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.Entities
{
    public abstract class BaseGameSettingsModel : BaseEntityModel
    {

        #region Timeout Limits

        public int TotalTimeoutLimit { get; set; }

        public int FullTimeoutLimit { get; set; }

        public int ShortTimeoutLimit { get; set; }

        #endregion

        #region Foul Settings
        /// <summary>
        /// The number of personal fouls each player is allowed before fouling out 
        /// </summary>
        public int PersonalFoulLimit { get; set; } = 6;

        /// <summary>
        /// The number of technical fouls each player is allowed before fouling out 
        /// </summary>
        public int TechnicalFoulLimit { get; set; } = 2;

        /// <summary>
        /// The default shot clock duration in seconds
        /// </summary>
        #endregion

        #region Clock
        public bool IsTimed { get; set; }

        /// <summary>
        /// When set to true, the game clock isn't stopped following a successful field goal.
        /// </summary>
        public bool UsesRunningClock { get; set; } = true;

        /// <summary>
        /// In minutes
        /// </summary>
        public int OvertimePeriodDuration { get; set; }
        public int? DefaultShotClockDuration { get; set; } = 24;
        #endregion

        /// <summary>
        /// The amount of time (in seconds) the shot clock will reset to after an offensive rebound
        /// </summary>
        public int? OffensiveRebondShotClockDuration { get; set; } = 14;

        #region Period and Durations
        public int NumberOfRegulationPeriods { get; set; } = 4;

        /// <summary>
        /// In minutes
        /// </summary>
        public int RegulationPeriodDuration { get; set; }
        #endregion Period and Durations

    }
}
