using Dribbly.Model.Games;
using Dribbly.Model.Teams;
using System.Collections.Generic;

namespace Dribbly.Model.Entities
{
    public class UpdateGameEventResultModel
    {
        /// <summary>
        /// The updated event
        /// </summary>
        public GameEventModel Event { get; set; }
        public GameModel Game { get; set; }
        /// <summary>
        /// The collection of teams that were affected
        /// </summary>
        public List<GameTeamModel> Teams { get; set; } = new List<GameTeamModel>();
        /// <summary>
        /// The list of players that were affected
        /// </summary>
        public List<GamePlayerModel> Players { get; set; } = new List<GamePlayerModel>();

        /// <summary>
        /// Whether or not the update is DELETE
        /// </summary>
        public bool IsDelete { get; set; }
    }
}
