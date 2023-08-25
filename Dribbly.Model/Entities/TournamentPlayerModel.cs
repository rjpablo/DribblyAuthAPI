using Dribbly.Model.Account;
using Dribbly.Model.Tournaments;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    [Table("TournamentPlayers")]
    public class TournamentPlayerModel : BaseStatsSummaryModel
    {
        // Composiste key: {PlayerId, TournamentId}
        // Defined in OnModelCreating method

        [ForeignKey(nameof(Account))]
        public long AccountId { get; set; }
        [ForeignKey(nameof(Tournament))]
        public long TournamentId { get; set; }
        [NotMapped]
        public new long Id { get; set; }
        [NotMapped]
        public new DateTime DateAdded { get; set; }
        public int? JerseyNo { get; set; }

        #region Navigational Properties
        [JsonIgnore]
        public TournamentModel Tournament { get; set; }
        public PlayerModel Account { get; set; }
        #endregion
    }
}
