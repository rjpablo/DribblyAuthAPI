using Dribbly.Core.Models;

namespace Dribbly.Model.DTO
{
    public class GetTournamentPlayersInputModel : PagedGetInputModel
    {
        public long TournamentId { get; set; }
    }
}
