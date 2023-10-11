using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.DTO
{
    public class GetAddGameModalInputModel
    {
        public long? TournamentId { get; set; }
        public long? CourtId { get; set; }
    }
}
