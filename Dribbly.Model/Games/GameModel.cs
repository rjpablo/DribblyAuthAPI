using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Courts;
using Dribbly.Model.Shared;
using Dribbly.Model.Teams;
using Dribbly.Model.Tournaments;
using Dribbly.Service.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Games
{
    // Games table has an additional ID column which is a Foreign Key to the bookings table
    [Table("Games")]
    public class GameModel : BaseEntityModel, IIndexedEntity
    {
        #region MappedColumns
        [Required]
        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }

        [MinLength(5)]
        public string Title { get; set; }

        [Required]
        public long AddedById { get; set; }

        [ForeignKey(nameof(Court)), Required]
        public long CourtId { get; set; }

        public GameStatusEnum Status { get; set; }

        [ForeignKey(nameof(Team1))]
        public long? Team1Id { get; set; }

        [ForeignKey(nameof(Team2))]
        public long? Team2Id { get; set; }

        public int Team1Score { get; set; } = 0;

        public int Team2Score { get; set; } = 0;

        public long? WinningTeamId { get; set; }
        [ForeignKey(nameof(Tournament))]
        public long? TournamentId { get; set; }

        public int CurrentPeriod { get; set; } = 1;

        /// <summary>
        /// The remaining time (in milliseconds) in the current period
        /// </summary>
        public int RemainingTime { get; set; }

        /// <summary>
        /// The timestamp when the remaining time was last updated
        /// </summary>
        public DateTime RemainingTimeUpdatedAt { get; set; }

        public bool IsTimed { get; set; }

        #endregion MappedColumns

        #region Unmapped Columns
        [NotMapped]
        public EntityTypeEnum EntityType { get; } = EntityTypeEnum.Game;

        [NotMapped]
        public string Name { get { return Title; } }
        public EntityStatusEnum EntityStatus { get; set; }
        [NotMapped]
        public string IconUrl { get { return ""; } }
        [NotMapped]
        public string Description { get { return ""; } }
        #endregion

        #region Navigational Properties
        public virtual CourtModel Court { get; set; }

        public virtual AccountBasicInfoModel AddedBy { get; set; }

        public virtual TeamModel Team1 { get; set; }

        public virtual TeamModel Team2 { get; set; }

        [JsonIgnore] // To avoid "self-referencing loop detected" error
        public TournamentModel Tournament { get; set; }

        #endregion
    }
}