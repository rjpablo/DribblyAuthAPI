using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    [Table("StageTeams")]
    public class StageTeamModel: BaseTeamStatsModel
    {
        public long StageId { get; set; }
        public long? BracketId { get; set; }

        public TournamentStageModel Stage { get; set; }
        public StageBracketModel Bracket { get; set; }
    }
}
