namespace Dribbly.Model.Teams
{
    public class ProcessJoinTeamRequestInputModel
    {
        public JoinTeamRequestModel Request { get; set; }
        public bool ShouldApprove { get; set; }
    }
}
