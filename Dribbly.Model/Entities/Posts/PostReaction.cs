using Dribbly.Model.Posts;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities.Posts
{
    [Table("PostReactions")]
    public class PostReaction : UserReactionModel
    {
        [ForeignKey(nameof(Post))]
        public long PostId { get; set; }
        [JsonIgnore]
        public PostModel Post { get; set; }
    }
}
