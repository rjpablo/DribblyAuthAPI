using Dribbly.Core.Models;
using Dribbly.Model.Posts;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    public class PostFile
    {
        [Key, Column(Order=1)]
        [ForeignKey(nameof(Post))]
        public long PostId { get; set; }
        [Key, Column(Order = 2)]
        [ForeignKey(nameof(File))]
        public long FileId { get; set; }
        public int Order { get; set; }
        [JsonIgnore]
        public PostModel Post { get; set; }
        public MultimediaModel File { get; set; }
    }
}
