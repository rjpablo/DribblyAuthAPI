using Dribbly.Model.Courts;
using Dribbly.Model.Teams;
using System.Collections.Generic;
using System.Linq;

namespace Dribbly.Service.DTO
{
    public class GameTeam
    {
        public long GameId { get; set; }
        public long TeamId { get; set; }
        public string TeamName { get; set; }
        public string ShortName { get; set; }
        public PhotoModel Logo { get; set; }
        public List<GamePlayer> Players { get; set; }

        public GameTeam(TeamModel team, long gameId)
        {
            GameId = gameId;
            TeamId = team.Id;
            TeamName = team.Name;
            ShortName = team.ShortName;
            Logo = team.Logo;
            Players = team.Members.Select(m=>new GamePlayer(m)).ToList();
        }
    }
}