using Dribbly.Core.Models;
using Dribbly.Model.Teams;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    public abstract class BaseTeamStatsModel : BaseStatsSummaryModel
    {
        [ForeignKey(nameof(Team))]
        public long TeamId { get; set; }

        public TeamModel Team { get; set; }
    }
}
