namespace Dribbly.Model.Teams
{
    public class UserTeamRelationModel
    {
        public bool IsFollowing { get; set; }
        public bool IsOwner { get; set; }
        public bool HasPendingJoinRequest { get; set; }
    }
}
