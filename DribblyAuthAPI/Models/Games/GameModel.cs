using DribblyAuthAPI.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace DribblyAuthAPI.Models.Games
{
    [Table("Games")]
    public class GameModel : EventModel
    {
        public long Team1Id { get; set; }
        public long Team2Id { get; set; }
        public long WinningTeamId { get; set; }
    }
}