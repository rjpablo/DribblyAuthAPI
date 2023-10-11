using Dribbly.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.DTO
{
    public class GetTournamentTeamsInputModel: PagedGetInputModel
    {
        public long TournamentId { get; set; }
    }
}
