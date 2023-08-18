using Dribbly.Model.Enums;
using System.Collections.Generic;

namespace Dribbly.Model.GameEvents
{
    public class UpdateGameEventInputModel
    {
        public long Id { get; set; }
        public long GameId { get; set; }
        public long? TeamId { get; set; }
        public long? PerformedById { get; set; }
        public GameEventTypeEnum Type { get; set; }
        public int? Period { get; set; }
        /// <summary>
        /// The time on the game clock when the event happened
        /// </summary>
        public int? ClockTime { get; set; }
        /// <summary>
        /// The id of the shot linked to this event, if any
        /// </summary>
        public long? ShotId { get; set; }

        public bool IsNew { get; set; }

        #region Shot Properties
        public int Points { get; set; }
        public bool IsMiss { get; set; }
        #endregion

        #region Foul Properties
        public int? FoulId { get; set; }
        #endregion
    }
}
