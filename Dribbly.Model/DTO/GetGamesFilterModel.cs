using Dribbly.Core.Models;
using System.Collections.Generic;

namespace Dribbly.Model.DTO
{
    public class GetGamesFilterModel : PagedGetInputModel
    {
        public bool UpcomingOnly { get; set; }
        public List<long> TeamIds { get; set; }

    }
}
