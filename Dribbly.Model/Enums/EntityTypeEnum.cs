using Dribbly.Model.Account;
using Dribbly.Model.Courts;
using Dribbly.Model.Games;
using Dribbly.Model.Leagues;
using Dribbly.Model.Posts;
using Dribbly.Model.Tournaments;

namespace Dribbly.Service.Enums
{
    public enum EntityTypeEnum
    {
        /// <summary>
        /// <see cref="AccountModel"/>
        /// </summary>
        Account = 0,
        /// <summary>
        /// <see cref="CourtModel"/>
        /// </summary>
        Court = 1,
        /// <summary>
        /// <see cref="GameModel"/>
        /// </summary>
        Game = 2,
        // TODO link TeamModel when added
        Team = 3,
        /// <summary>
        /// <see cref="PostModel"/>
        /// </summary>
        Post = 4,
        /// <summary>
        /// <see cref="LeagueModel"/>
        /// </summary>
        League = 5,
        /// <summary>
        /// <see cref="TournamentModel"/>
        /// </summary>
        Tournament = 6
    }
}