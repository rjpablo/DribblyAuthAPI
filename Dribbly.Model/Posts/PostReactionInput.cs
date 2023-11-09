using Dribbly.Model.Enums;

namespace Dribbly.Model.Posts
{
    public class PostReactionInput
    {
        public long PostId { get; set; }
        public ReactionTypeEnum ReactionType { get; set; }
    }
}
