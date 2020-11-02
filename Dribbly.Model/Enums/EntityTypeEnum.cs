using Dribbly.Model.Account;
using Dribbly.Model.Courts;
using Dribbly.Model.Games;
using Dribbly.Model.Posts;

namespace Dribbly.Service.Enums
{
    public enum EntityTypeEnum
    {
        /// <summary>
        /// <see cref="AccountModel"/>
        /// </summary>
        Account,
        /// <summary>
        /// <see cref="CourtModel"/>
        /// </summary>
        Court,
        /// <summary>
        /// <see cref="GameModel"/>
        /// </summary>
        Game,
        // TODO link TeamModel when added
        Team,
        /// <summary>
        /// <see cref="PostModel"/>
        /// </summary>
        Post
    }
}