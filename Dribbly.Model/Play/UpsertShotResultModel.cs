using Dribbly.Model.Fouls;
using Dribbly.Model.Games;

namespace Dribbly.Model.Play
{
    public class UpsertShotResultModel
    {
        public GameModel Game { get; set; }
        public UpsertFoulResultModel FoulResult { get; set; }
        /// <summary>
        /// The player's total number of points
        /// </summary>
        public int TotalPoints { get; set; }
    }
}
