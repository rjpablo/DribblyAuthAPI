using Dribbly.Model.Courts;
using Dribbly.Model.Entities;
using Dribbly.Model.Games;
using System.Runtime.Serialization;

namespace Dribbly.Model.DTO
{
    [DataContract]
    public class PlayerStatsViewModel
    {
        #region Account Info
        [DataMember]
        public long AccountId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public PhotoModel ProfilePhoto { get; set; }
        [DataMember]
        public int? JerseyNo { get; set; }

        #endregion

        #region Stats

        [DataMember]
        /// <summary>
        /// Games Played
        /// </summary>
        public int GP { get; set; }

        [DataMember]
        /// <summary>
        /// Games Won
        /// </summary>
        public int GW { get; set; }

        [DataMember]
        /// <summary>
        /// Average Points per Game
        /// </summary>
        public double PPG { get; set; }

        [DataMember]
        /// <summary>
        /// Rebounds per game
        /// </summary>
        public double RPG { get; set; }

        [DataMember]
        /// <summary>
        /// Assists per game
        /// </summary>
        public double APG { get; set; }

        [DataMember]
        /// <summary>
        /// Field Goal Percentage
        /// </summary>
        public double? FGP { get; set; }

        [DataMember]
        /// <summary>
        /// 3pt percentage
        /// </summary>
        public double? ThreePP { get; set; }

        [DataMember]
        /// <summary>
        /// Free Throw percentage
        /// </summary>
        public double? FTP { get; set; }

        [DataMember]
        /// <summary>
        /// Steals per game
        /// </summary>
        public double SPG { get; set; }

        [DataMember]
        /// <summary>
        /// Blocks per game
        /// </summary>
        public double BPG { get; set; }

        [DataMember]
        /// <summary>
        /// Minutes per game
        /// </summary>
        public double MPG { get; set; }

        [DataMember]
        public long LastGameId { get; set; }

        [DataMember]
        public GameModel LastGame { get; set; }
        #endregion

        public PlayerStatsViewModel(PlayerStatsModel source)
        {
            // Account Info
            AccountId = source.AccountId;
            Name = source.Account.Name;
            Username = source.Account.Username;
            ProfilePhoto = source.Account.ProfilePhoto;

            // Stats
            GP = source.GP;
            GW = source.GW;
            PPG = source.PPG;
            RPG = source.RPG;
            APG = source.APG;
            FGP = source.FGP;
            ThreePP = source.ThreePP;
            FTP = source.FTP;
            BPG = source.BPG;
            SPG = source.SPG;
            MPG = source.MPG;
            LastGameId = source.LastGameId;
            LastGame = source.LastGame;
        }

        public PlayerStatsViewModel(TournamentPlayerModel source)
        {
            // Account Info
            AccountId = source.AccountId;
            Name = source.Account.Name;
            Username = source.Account.Username;
            ProfilePhoto = source.Account.ProfilePhoto;
            JerseyNo = source.JerseyNo;

            // Stats
            GP = source.GP;
            GW = source.GW;
            PPG = source.PPG;
            RPG = source.RPG;
            APG = source.APG;
            FGP = source.FGP;
            ThreePP = source.ThreePP;
            BPG = source.BPG;
            SPG = source.SPG;
            FTP = source.FTP;
        }
    }
}
