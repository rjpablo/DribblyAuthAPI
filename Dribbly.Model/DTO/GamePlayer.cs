using Dribbly.Core.Models;
using Dribbly.Model.Entities;
using Dribbly.Model.Games;
using Dribbly.Model.Teams;

namespace Dribbly.Service.DTO
{
    public class GamePlayer: IBaseStatsModel
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string Name { get; set; }
        public int JerseyNo { get; set; }
        #region Stats
        public int Points { get; set; }
        /// <summary>
        /// field goal attempts
        /// </summary>
        public int FGA { get; set; }
        /// <summary>
        /// field goals made
        /// </summary>
        public int FGM { get; set; }
        /// <summary>
        /// free throw attempts
        /// </summary>
        public int FTA { get; set; }
        /// <summary>
        /// free throws made
        /// </summary>
        public int FTM { get; set; }
        /// <summary>
        /// 3pt attempts
        /// </summary>
        public int ThreePA { get; set; }
        /// <summary>
        /// 3pts made
        /// </summary>
        public int ThreePM { get; set; }
        public int Blocks { get; set; }
        public int Rebounds { get; set; }
        public int Assists { get; set; }
        public int Turnovers { get; set; }
        public int Steals { get; set; }
        public bool? Won { get; set; }
        #endregion
        public int Fouls { get; set; }
        public long TeamId { get; set; }
        public bool IsEjected { get; set; }
        public bool HasFouledOut { get; set; }
        public MultimediaModel ProfilePhoto { get; set; }
        public GameModel Game { get; set; }

        public GamePlayer(TeamMembershipModel player)
        {
            Id = player.Account.IdentityUserId;
            Name = player.Account.Name;
            JerseyNo = player.JerseyNo;
            TeamId = player.TeamId;
            ProfilePhoto = player.Account.ProfilePhoto;
        }

        public GamePlayer(GamePlayerModel entity)
        {
            AccountId = entity.AccountId;
            Name = entity.Name;
            JerseyNo = entity.TeamMembership.JerseyNo;
            TeamId = entity.TeamMembership.TeamId;
            Game = entity.Game;
            HasFouledOut = entity.HasFouledOut;
            Points = entity.Points;
            FGA = entity.FGA;
            FGM = entity.FGM;
            FTA = entity.FTA;
            FTM = entity.FTM;
            ThreePA = entity.ThreePA;
            ThreePM = entity.ThreePM;
            Blocks = entity.Blocks;
            Rebounds = entity.Rebounds;
            Assists = entity.Assists;
            Turnovers = entity.Turnovers;
            Steals = entity.Steals;
            Won = entity.Won;
        }
    }
}