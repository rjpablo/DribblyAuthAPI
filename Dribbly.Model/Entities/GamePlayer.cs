using Dribbly.Core.Models;
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
        public string Name { get => TeamMembership.Name; }
        [NotMapped]
        public int JerseyNo { get => TeamMembership.JerseyNo; }
        public int Points { get; set; }
        public int Rebounds { get; set; }
        public int Fouls { get; set; }
        public int Blocks { get; set; }
        public int Assists { get; set; }
        [ForeignKey(nameof(GameTeam))]
        public long GameTeamId { get; set; }
        public bool HasFouledOut { get; set; }
        public bool IsInGame { get; set; }
        public EjectionStatusEnum EjectionStatus { get; set; } = EjectionStatusEnum.NotEjected;
        public PhotoModel ProfilePhoto { get => TeamMembership.Account.ProfilePhoto; }
        public TeamMembershipModel TeamMembership { get; set; }
        [JsonIgnore]
        public GameModel Game { get; set; }
        [JsonIgnore]
        public GameTeamModel GameTeam { get; set; }
    }
}