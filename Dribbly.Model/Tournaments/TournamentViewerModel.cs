using Dribbly.Core.Models;
using Dribbly.Model.Courts;
using Dribbly.Model.DTO;
using Dribbly.Model.Entities;
using Dribbly.Model.Enums;
using Dribbly.Model.Games;
using Dribbly.Model.Teams;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.Tournaments
{
    public class TournamentViewerModel : BaseGameSettingsModel
    {
        public string Name { get; set; }
        public long AddedById { get; set; }
        public TournamentStatusEnum Status { get; set; }
        public PhotoModel Logo { get; set; }
        public List<GameModel> Games { get; set; } = new List<GameModel>();
        public List<BaseTeamStatsModel> Teams { get; set; } = new List<BaseTeamStatsModel>();
        public IEnumerable<TournamentStageModel> Stages { get; set; } = new List<TournamentStageModel>();
        public List<JoinTournamentRequestModel> JoinRequests { get; set; } = new List<JoinTournamentRequestModel>();
        public CourtModel DefaultCourt { get; set; }

        public TournamentViewerModel(TournamentModel model)
        {
            DateAdded = model.DateAdded;
            Id = model.Id;
            Name = model.Name;
            AddedById = model.AddedById;
            Logo = model.Logo;
            Status = model.Status;
            Games = model.Games.ToList();
            Teams = model.Teams.ToList<BaseTeamStatsModel>();
            Stages = model.Stages;
            JoinRequests = model.JoinRequests.ToList();
            DefaultCourt = model.DefaultCourt;

            // Timeout Limits
            TotalTimeoutLimit = model.TotalTimeoutLimit;
            FullTimeoutLimit = model.FullTimeoutLimit;
            ShortTimeoutLimit = model.ShortTimeoutLimit;

            // Foul Settings
            PersonalFoulLimit = model.PersonalFoulLimit;
            TechnicalFoulLimit = model.TechnicalFoulLimit;

            // Clock
            IsTimed = model.IsTimed;
            UsesRunningClock = model.UsesRunningClock;
            OvertimePeriodDuration = model.OvertimePeriodDuration;
            DefaultShotClockDuration = model.DefaultShotClockDuration;
            OffensiveRebondShotClockDuration = model.OffensiveRebondShotClockDuration;

            // Period and Durations
            NumberOfRegulationPeriods = model.NumberOfRegulationPeriods;
            RegulationPeriodDuration = model.RegulationPeriodDuration;
        }
    }
}
