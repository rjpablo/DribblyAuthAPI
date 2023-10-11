using Dribbly.Core.Enums;
using Dribbly.Core.Models;
using Dribbly.Model.Courts;
using Dribbly.Model.DTO;
using Dribbly.Model.Entities;
using Dribbly.Model.Enums;
using Dribbly.Model.Games;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Tournaments
{
    [Table("Tournaments")]
    public class TournamentModel : BaseGameSettingsModel, IIndexedEntity
    {
        public string Name { get; set; }
        public long AddedById { get; set; }
        public TournamentStatusEnum Status { get; set; }
        public ICollection<GameModel> Games { get; set; } = new List<GameModel>();
        public ICollection<TournamentTeamModel> Teams { get; set; } = new List<TournamentTeamModel>();
        public ICollection<TournamentStageModel> Stages { get; set; } = new List<TournamentStageModel>();
        public ICollection<JoinTournamentRequestModel> JoinRequests { get; set; } = new List<JoinTournamentRequestModel>();
        public long? LogoId { get; set; }

        #region Settings
        public long? DefaultCourtId { get; set; }

        #endregion

        public EntityStatusEnum EntityStatus { get; set; }

        #region Navigational Properties
        public CourtModel DefaultCourt { get; set; }
        public MultimediaModel Logo { get; set; }
        #endregion

        [NotMapped]
        public string IconUrl { get { return Logo?.Url; } }

        [NotMapped]
        public EntityTypeEnum EntityType { get; } = EntityTypeEnum.Court;

        [NotMapped]
        public string Description { get; set; }

        public TournamentModel()
        {
            EntityType = EntityTypeEnum.Tournament;
        }

        public void OverrideSettings(UpdateTournamentSettingsModel source)
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

            DefaultCourtId = source.DefaultCourtId;
        }
    }
}
