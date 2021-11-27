using Dribbly.Service.Enums;

namespace Dribbly.Model.Teams
{
    public class JoinTeamRequestInputModel
    {
        public int TeamId { get; set; }
        public PlayerPositionEnum Position { get; set; }
    }
}
