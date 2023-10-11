using Dribbly.Model.Entities;
using System.Collections.Generic;

namespace Dribbly.Model.GameEvents
{
    public class UpsertFreeThrowInputModel: UpdateGameEventInputModel
    {
        public List<bool> AttemptResults { get; set; }
        public GameEventModel Rebound { get; set; }
    }
}
