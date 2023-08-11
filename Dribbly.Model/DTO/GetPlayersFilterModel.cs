using Dribbly.Core.Models;
using System.Collections.Generic;

namespace Dribbly.Model.DTO
{
    public class GetPlayersFilterModel : PagedGetInputModel
    {
        public List<long> CourdIds { get; set; } = new List<long>();

    }
}
