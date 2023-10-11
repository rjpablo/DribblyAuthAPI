namespace Dribbly.Model.Teams
{
    public interface ITeamMemberListItem
    {
        bool IsCurrentMember { get; }
        bool HasPendingJoinRequest { get; }
        bool IsFormerMember { get; }
        string PrimaryPhotoUrl { get; }
    }
}
