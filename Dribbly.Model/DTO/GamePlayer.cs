using Dribbly.Core.Models;
using Dribbly.Model.Teams;

namespace Dribbly.Service.DTO
{
    public class GamePlayer
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int JerseyNo { get; set; }
        public int Points { get; set; }
        public int Rebounds { get; set; }
        public int Fouls { get; set; }
        public int Assists { get; set; }
        public long TeamId { get; set; }
        public bool IsEjected { get; set; }
        public bool HasFouledOut { get; set; }
        public PhotoModel ProfilePhoto { get; set; }

        public GamePlayer(TeamMembershipModel player)
        {
            Id = player.Account.IdentityUserId;
            Name = player.Account.Name;
            JerseyNo = player.JerseyNo;
            TeamId = player.TeamId;
            ProfilePhoto = player.Account.ProfilePhoto;
        }
    }
}