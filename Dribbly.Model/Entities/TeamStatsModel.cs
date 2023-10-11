using Dribbly.Model.Teams;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.Entities
{
    [Table("TeamStats")]
    public class TeamStatsModel : BaseTeamStatsModel
    {
        [ForeignKey(nameof(Team))]
        public new long TeamId
        {
            get { return base.TeamId; }
            set { base.TeamId = value; }
        }

        [NotMapped]
        public new long Id
        {
            get { return base.Id; }
            set { base.Id = value; }
        }

        [NotMapped]
        public new DateTime DateAdded
        {
            get { return base.DateAdded; }
            set { base.DateAdded = value; }
        }
    }
}
