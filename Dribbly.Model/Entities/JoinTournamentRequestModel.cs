using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Teams;
using Dribbly.Model.Tournaments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    [Table("JoinTournamentRequests")]
    public class JoinTournamentRequestModel:BaseEntityModel
    {
        [ForeignKey(nameof(Tournament))]
        public long TournamentId { get; set; }

        [ForeignKey(nameof(Team))]
        public long TeamId { get; set; }
        [ForeignKey(nameof(AddedBy))]

        public long AddedByID { get; set; }

        #region Navigational Properties
        public TournamentModel Tournament { get; set; }
        public TeamModel Team { get; set; }
        public AccountModel AddedBy { get; set; }
        #endregion
    }
}
