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

        public GameModel ToGameModel()
        {
            return new GameModel
            {
                Start = Start,
                Title = Title,
                CourtId = CourtId
            };
        }
    }
}