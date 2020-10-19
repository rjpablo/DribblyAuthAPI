using Dribbly.Model.Bookings;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Games
{
    // Games table has an additional ID column which is a Foreign Key to the bookings table
    [Table("Games")]
    public class GameModel : BookingModel
    {
        public long Team1Id { get; set; }
        public long Team2Id { get; set; }
        public long WinningTeamId { get; set; }
    }
}