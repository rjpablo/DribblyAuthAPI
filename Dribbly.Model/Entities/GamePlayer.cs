using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Courts;
using Dribbly.Model.Enums;
using Dribbly.Model.Games;
using Dribbly.Model.Teams;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    [Table("GamePlayers")]
    public class GamePlayerModel : BaseStatsModel
    {
        /**
        * <summary>
        * Links to <see cref="TeamMembershipModel">TeamMembershipModel</see>
        * </summary>
        */
        [ForeignKey(nameof(TeamMembership))]
        public long PlayerId { get; set; }
        [ForeignKey(nameof(Game))]
        public long GameId { get; set; }
        [NotMapped]
        public string Name { get => TeamMembership?.Name; }
        [NotMapped]
        public int? JerseyNo { get => TeamMembership?.JerseyNo; }
        [ForeignKey(nameof(Account))]
        public long AccountId { get; set; }

        #region Stats
        /// <summary>
        /// Offensive Rebounds
        /// </summary>
        public int OReb { get; set; }
        /// <summary>
        /// Defensive Rebounds
        /// </summary>
        public int DReb { get; set; }
        public int Fouls { get; set; }
        /// <summary>
        /// Free Throw Attempts
        /// </summary>
        public int FTA { get; set; }
        /// <summary>
        /// Free Throws made
        /// </summary>
        public int FTM { get; set; }
        /// <summary>
        /// Playing time in milliseconds
        /// </summary>
        public int PlayTimeMs { get; set; }

        #endregion

        [ForeignKey(nameof(GameTeam))]
        public long GameTeamId { get; set; }
        public bool HasFouledOut { get; set; }
        public bool IsInGame { get; set; }
        public EjectionStatusEnum EjectionStatus { get; set; } = EjectionStatusEnum.NotEjected;
        public PhotoModel ProfilePhoto { get => TeamMembership?.Account?.ProfilePhoto; }
        public TeamMembershipModel TeamMembership { get; set; }
        [JsonIgnore]
        public GameModel Game { get; set; }
        [JsonIgnore]
        public GameTeamModel GameTeam { get; set; }
        public AccountModel Account { get; set; }
    }
}