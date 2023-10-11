using Dribbly.Core.Models;
using Dribbly.Model.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dribbly.Model.Games
{
    // Games table has an additional ID column which is a Foreign Key to the bookings table
    public class AddGameInputModel : BaseGameSettingsModel
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

        public GameModel ToGameModel()
        {
            var game = new GameModel
            {
                Start = Start,
                Title = Title,
                CourtId = CourtId,
                TournamentId = TournamentId,
                StageId = StageId,
                BracketId = BracketId
            };

            game.OverrideSettings(this);

            return game;
        }
    }
}