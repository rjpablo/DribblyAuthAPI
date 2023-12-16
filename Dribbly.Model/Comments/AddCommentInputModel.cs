using Dribbly.Core.Enums;

namespace Dribbly.Model.Comments
{
    public class AddCommentInputModel
    {
        public EntityTypeEnum CommentedOnType { get; set; }
        public long CommentedOnId { get; set; }
        public string Message { get; set; }
    }
}
