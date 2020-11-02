using Dribbly.Service.Enums;
using System.ComponentModel.DataAnnotations;

namespace Dribbly.Model.Posts
{
    public class GetPostsInputModel
    {
        [Required]
        public EntityTypeEnum PostedOnType { get; set; }

        [Required]
        public string PostedOnId { get; set; }

        public int GetCount { get; set; }

        public long? CeilingPostId { get; set; }
    }
}