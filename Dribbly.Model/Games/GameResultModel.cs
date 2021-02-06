namespace Dribbly.Model.Games
{
    public class GameResultModel
    {
        public long GameId { get; set; }

        public int? Team1Score { get; set; }

        public int? Team2Score { get; set; }

        public long? WinningTeamId { get; set; }
    }
}