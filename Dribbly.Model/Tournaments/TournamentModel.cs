using Dribbly.Core.Models;
using Dribbly.Model.Enums;
using Dribbly.Model.Games;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.Tournaments
{
    [Table("Tournaments")]
    public class TournamentModel : BaseEntityModel
    {
        public string Name { get; set; }
        public long AddedById { get; set; }
        public TournamentStatusEnum Status { get; set; }
        public ICollection<GameModel> Games { get; set; } = new List<GameModel>();
    }
}
