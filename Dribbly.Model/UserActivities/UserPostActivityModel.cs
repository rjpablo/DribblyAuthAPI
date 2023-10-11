using Dribbly.Model.Posts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.UserActivities
{
    [Table("UserPostActivities")]
    public class UserPostActivityModel : UserActivityModel
    {
        [ForeignKey(nameof(Post))]
        public long PostId { get; set; }

        public PostModel Post { get; set; }
    }
}
