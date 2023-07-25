using Dribbly.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dribbly.Model.Games
{
    // Games table has an additional ID column which is a Foreign Key to the bookings table
    public class AddGameInputModel : BaseEntityModel
    {
        [Required]
        public DateTime Start { get; set; }

        [MinLength(5)]
        public string Title { get; set; }

        public long CourtId { get; set; }

        public long Team1Id { get; set; }

        public long Team2Id { get; set; }

        public bool IsTeam1Open { get; set; }

        public bool IsTeam2Open { get; set; }
        public long? TournamentId { get; set; }
        public long? StageId { get; set; }
        public long? BracketId { get; set; }

        public bool IsTimed { get; set; }
        public int? DefaultShotClockDuration { get; set; } = 24;
        public int? OffensiveRebondShotClock { get; set; } = 14;
        public int NumberOfRegulationPeriods { get; set; }

        /// <summary>
        /// In minutes
        /// </summary>
        public int RegulationPeriodDuration { get; set; }
        /// <summary>
        /// In minutes
        /// </summary>
        public int OvertimePeriodDuration { get; set; }

        public bool UsesRunningClock { get; set; }

        public GameModel ToGameModel()
        {
            return new GameModel
            {
                Start = Start,
                Title = Title,
                CourtId = CourtId,
                TournamentId = TournamentId,
                StageId = StageId,
                BracketId = BracketId,
                IsTimed = IsTimed,
                DefaultShotClockDuration = DefaultShotClockDuration,
                OffensiveRebondShotClock = OffensiveRebondShotClock,
                NumberOfRegulationPeriods = NumberOfRegulationPeriods,
                RegulationPeriodDuration = RegulationPeriodDuration,
                OvertimePeriodDuration = OvertimePeriodDuration,
                UsesRunningClock = UsesRunningClock
            };
        }
    }
}