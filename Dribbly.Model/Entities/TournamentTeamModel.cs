using Dribbly.Core.Models;
using Dribbly.Model.Teams;
using Dribbly.Model.Tournaments;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    [Table("TournamentTeams")]
    public class TournamentTeamModel : BaseTeamStatsModel
    {
        [ForeignKey(nameof(Tournament))]
        public long TournamentId { get; set; }

        #region Navigational Properties
        [JsonIgnore]
        public TournamentModel Tournament { get; set; }
        #endregion
    }
}
