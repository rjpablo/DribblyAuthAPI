using Dribbly.Model.Entities;
using Dribbly.Model.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.Play
{
    public class ShotDetailsInputModel
    {
        public ShotModel Shot { get; set; }
        public bool WithFoul { get; set; }
        public MemberFoulModel Foul { get; set; }
        public bool WithBlock { get; set; }
        public GameEventModel Block { get; set; }
        public bool WithAssist { get; set; }
        public GameEventModel Assist { get; set; }
        public bool WithRebound { get; set; }
        public GameEventModel Rebound { get; set; }
        public bool IsEdit { get; set; }
    }
}
