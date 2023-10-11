using Dribbly.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.Games
{
    public class StartGameInputModel
    {
        public long GameId { get; set; }
        public GameEventModel Jumpball { get; set; }
        public DateTime StartedAt { get; set; }
    }
}
