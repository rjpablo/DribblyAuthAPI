using Dribbly.Model.Enums;

namespace Dribbly.Model.Comments
{
    public class AddCommentInputModel
    {
        public CommentedOnTypeEnum CommentedOnType { get; set; }
        public long CommentedOnId { get; set; }
        public string Message { get; set; }
    }
}
