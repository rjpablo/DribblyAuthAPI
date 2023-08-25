using Dribbly.Core.Models;
using Dribbly.Model.Entities;

namespace Dribbly.Model.Games
{
    public class AddGameModalModel: BaseGameSettingsModel
    {
        public ChoiceItemModel<long> CourtChoice { get; set; }

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
