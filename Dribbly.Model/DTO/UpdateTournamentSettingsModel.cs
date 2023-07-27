using Dribbly.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.DTO
{
    public class UpdateTournamentSettingsModel: BaseGameSettingsModel
    {
        public long? DefaultCourtId { get; set; }
        public bool ApplyToGames { get; set; }
    }
}
