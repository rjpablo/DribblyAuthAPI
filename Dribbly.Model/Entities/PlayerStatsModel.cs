using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Games;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    [Table("PlayerStats")]
    public class PlayerStatsModel : BaseModel
    {
        [Key]
        [ForeignKey(nameof(Account))]
        public long AccountId { get; set; }

        /// <summary>
        /// Games Played
        /// </summary>
        public int GP { get; set; }

        /// <summary>
        /// Games Won
        /// </summary>
        public int GW { get; set; }

        /// <summary>
        /// Average Points per Game
        /// </summary>
        public double PPG { get; set; }

        /// <summary>
        /// Rebounds per game
        /// </summary>
        public double RPG { get; set; }

        /// <summary>
        /// Assists per game
        /// </summary>
        public double APG { get; set; }

        /// <summary>
        /// Field Goal Percentage
        /// </summary>
        public double FGP { get; set; }

        /// <summary>
        /// 3pt percentage
        /// </summary>
        public double ThreePP { get; set; }

        /// <summary>
        /// Free Throw percentage
        /// </summary>
        public double FTP { get; set; }

        /// <summary>
        /// Blocks per game
        /// </summary>
        public double BPG { get; set; }

        /// <summary>
        /// Minutes per game
        /// </summary>
        public double MPG { get; set; }

        [ForeignKey(nameof(LastGame))]
        public long LastGameId { get; set; }

        #region Navigational Properties
        public AccountModel Account { get; set; }
        public GameModel LastGame { get; set; }
        #endregion

        public PlayerStatsModel() { }

        public PlayerStatsModel(long accountId)
        {
            AccountId = accountId;
        }
    }
}
