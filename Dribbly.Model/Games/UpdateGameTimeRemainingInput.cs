using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.Games
{
    public class UpdateGameTimeRemainingInput
    {
        public long GameId { get; set; }
        public int TimeRemaining { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsLive { get; set; }
    }
}
