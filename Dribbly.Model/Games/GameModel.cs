using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Courts;
using Dribbly.Model.Entities;
using Dribbly.Model.Shared;
using Dribbly.Model.Teams;
using Dribbly.Model.Tournaments;
using Dribbly.Service.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        /// 0 = Not Set
        /// 1 = Team1
        /// 2 = Team2
        /// </summary>
        public int NextPossession { get; set; }

        /// <summary>
        /// The remaining time (in milliseconds) in the current period
        /// </summary>
        public int RemainingTime { get; set; }

        /// <summary>
        /// The timestamp when the remaining time was last updated
        /// </summary>
        public DateTime? RemainingTimeUpdatedAt { get; set; }

        /// <summary>
        /// The remaining time (in milliseconds) in the shot clock
        /// </summary>
        public int? RemainingShotTime { get; set; }

        /// <summary>
        /// The default shot clock duration in seconds
        /// </summary>
        public int? DefaultShotClockDuration { get; set; } = 24;

        /// <summary>
        /// The amount of time (in seconds) the shot clock will reset to after an offensive rebound
        /// </summary>
        public int? OffensiveRebondShotClock { get; set; } = 14;

        /// <summary>
        /// Whether or not the ball was live when the remainingTime was last updated
        /// </summary>
        public bool IsLive { get; set; }

        public bool IsTimed { get; set; }

        /// <summary>
        /// The number of personal fouls each player is allowed before fouling out 
        /// </summary>
        public int PersonalFoulLimit { get; set; } = 6;

        /// <summary>
        /// The number of technical fouls each player is allowed before fouling out 
        /// </summary>
        public int TechnicalFoulLimit { get; set; } = 2;

        #endregion MappedColumns

        #region Collections
        public ICollection<GameEventModel> GameEvents { get; set; }
        #endregion

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

        public virtual GameTeamModel Team1 { get; set; }

        public virtual GameTeamModel Team2 { get; set; }

        [JsonIgnore] // To avoid "self-referencing loop detected" error
        public TournamentModel Tournament { get; set; }

        #endregion
    }
}