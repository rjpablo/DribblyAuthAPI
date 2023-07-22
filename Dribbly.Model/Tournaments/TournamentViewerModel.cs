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
    public class TournamentViewerModel
    {
        public DateTime DateAdded { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public long AddedById { get; set; }
        public TournamentStatusEnum Status { get; set; }
        public List<GameModel> Games { get; set; } = new List<GameModel>();
        public List<TeamStatsViewModel> Teams { get; set; } = new List<TeamStatsViewModel>();
        public List<JoinTournamentRequestModel> JoinRequests { get; set; } = new List<JoinTournamentRequestModel>();
        public CourtModel DefaultCourt { get; set; }

        public TournamentViewerModel(TournamentModel model)
        {
            DateAdded = model.DateAdded;
            Id = model.Id;
            Name = model.Name;
            AddedById = model.AddedById;
            Status = model.Status;
            Games = model.Games.ToList();
            Teams = model.Teams.Select(t => new TeamStatsViewModel(t)).ToList();
            JoinRequests = model.JoinRequests.ToList();
            DefaultCourt = model.DefaultCourt;
        }
    }
}
