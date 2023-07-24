using System.Collections.Generic;

namespace Dribbly.Model.DTO
{
    public class SetStageTeamsInputModel
    {
        public long StageId { get; set; }
        public List<long> TeamIds { get; set; }
    }
}
