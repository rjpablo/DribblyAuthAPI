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
        [Key, ForeignKey(nameof(Team))]
        public new long TeamId { get; set; }

        [NotMapped]
        public new int Id { get; set; }

        [NotMapped]
        public new DateTime DateAdded { get; set; }
    }
}
