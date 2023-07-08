using Dribbly.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.Games
{
    [NotMapped]
    public class UpdateLineupInputModel : GameEventModel
    {
        public List<long> GamePlayerIds { get; set; }

        public UpdateLineupInputModel()
        {
            Type = Enums.GameEventTypeEnum.ChangeLineup;
        }
    }
}
