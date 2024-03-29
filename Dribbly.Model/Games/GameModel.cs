﻿using Dribbly.Core.Enums;
using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Courts;
using Dribbly.Model.Entities;
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
    public class GameModel : BaseGameSettingsModel, IIndexedEntity
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
        [ForeignKey(nameof(Stage))]
        public long? StageId { get; set; }
        [ForeignKey(nameof(Bracket))]
        public long? BracketId { get; set; }


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
        /// Whether or not the ball was live when the remainingTime was last updated
        /// </summary>
        public bool IsLive { get; set; }

        [ForeignKey(nameof(Timekeeper))]
        public long? TimekeeperId { get; set; }

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

        public AccountModel Timekeeper { get; set; }

        public virtual GameTeamModel Team1 { get; set; }

        public virtual GameTeamModel Team2 { get; set; }
        public TournamentModel Tournament { get; set; }

        [JsonIgnore]
        public TournamentStageModel Stage { get; set; }

        [JsonIgnore]
        public StageBracketModel Bracket { get; set; }

        #endregion

        public void OverrideSettings(BaseGameSettingsModel source)
        {
            // Timeout Limits
            TotalTimeoutLimit = source.TotalTimeoutLimit;
            FullTimeoutLimit = source.FullTimeoutLimit;
            ShortTimeoutLimit = source.ShortTimeoutLimit;

            // Foul Settings
            PersonalFoulLimit = source.PersonalFoulLimit;
            TechnicalFoulLimit = source.TechnicalFoulLimit;

            // Clock
            IsTimed = source.IsTimed;
            UsesRunningClock = source.UsesRunningClock;
            OvertimePeriodDuration = source.OvertimePeriodDuration;
            DefaultShotClockDuration = source.DefaultShotClockDuration;
            OffensiveRebondShotClockDuration = source.OffensiveRebondShotClockDuration;

            // Period and Durations
            NumberOfRegulationPeriods = source.NumberOfRegulationPeriods;
            RegulationPeriodDuration = source.RegulationPeriodDuration;
        }
    }
}