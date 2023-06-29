using Dribbly.Model.Fouls;
using Dribbly.Model.Games;

namespace Dribbly.Model.Play
{
    public class UpsertShotResultModel
    {
        public UpsertFoulResultModel FoulResult { get; set; }
        /// <summary>
        /// The player's total number of points
        /// </summary>
        public int TotalPoints { get; set; }
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
        public BlockResultModel BlockResult { get; set; }
    }
}
