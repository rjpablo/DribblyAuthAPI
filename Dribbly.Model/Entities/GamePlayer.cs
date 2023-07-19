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
    public class GamePlayerModel : BaseEntityModel
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
        public int Points { get; set; }
        public int Rebounds { get; set; }
        /// <summary>
        /// Offensive Rebounds
        /// </summary>
        public int OReb { get; set; }
        /// <summary>
        /// Defensive Rebounds
        /// </summary>
        public int DReb { get; set; }
        public int Fouls { get; set; }
        public int Blocks { get; set; }
        public int Assists { get; set; }
        /// <summary>
        /// Field Goal attempts
        /// </summary>
        public int FGA { get; set; }
        /// <summary>
        /// Field Goals Made
        /// </summary>
        public int FGM { get; set; }
        /// <summary>
        /// 3pt Attempts
        /// </summary>
        public int ThreePA { get; set; }
        /// <summary>
        /// 3pts Made
        /// </summary>
        public int ThreePM { get; set; }
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