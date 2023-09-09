using Dribbly.Core.Models;
using Dribbly.Model.Teams;
using Dribbly.Model.Tournaments;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    [Table("TournamentTeams")]
    public class TournamentTeamModel : BaseTeamStatsModel
    {
        // Composiste key: {TeamId, TournamentId}
        // Defined in OnModelCreating method
        [ForeignKey(nameof(Team))]
        public new long TeamId
        {
            get { return base.TeamId; }
            set { base.TeamId = value; }
        }

        [ForeignKey(nameof(Tournament))]
        public long TournamentId { get; set; }
        [NotMapped]
        public new long Id
        {
            get { return base.Id; }
            set { base.Id = value; }
        }

        #region Navigational Properties
        [JsonIgnore]
        public TournamentModel Tournament { get; set; }
        #endregion
    }
}
