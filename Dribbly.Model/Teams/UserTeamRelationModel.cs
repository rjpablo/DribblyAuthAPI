namespace Dribbly.Model.Teams
{
    public class UserTeamRelationModel
    {
        public bool IsFollowing { get; set; }
        public bool IsOwner { get; set; }
        public bool HasPendingJoinRequest { get; set; }
        public bool IsCurrentMember { get; set; }
        public bool IsFormerMember { get; set; }
        public bool IsCurrentCoach { get; set; }
        public bool Position { get; set; }
    }
}
