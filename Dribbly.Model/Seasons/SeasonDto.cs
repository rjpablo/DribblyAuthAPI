using Dribbly.Model.Enums;
using Dribbly.Model.Leagues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.Seasons
{
    public class SeasonDto
    {
        public long AddedById { get; set; }
        public string Name { get; set; }
        public long LeagueId { get; set; }
        public SeasonStatusEnum Status { get; set; }
    }
}
