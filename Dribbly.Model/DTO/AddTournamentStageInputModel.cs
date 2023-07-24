using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.DTO
{
    public class AddTournamentStageInputModel
    {
        public long TournamentId { get; set; }
        public string StageName { get; set; }
        public int BracketsCount { get; set; }
    }
}
