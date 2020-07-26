namespace Dribbly.Model.Shared
{
    public class FollowResultModel
    {
        public bool isSuccessful { get; set; }

        public bool isAlreadyFollowing { get; set; }

        public long newFollowerCount { get; set; }

        public bool IsNotCurrentlyFollowing { get; set; }
    }
}
