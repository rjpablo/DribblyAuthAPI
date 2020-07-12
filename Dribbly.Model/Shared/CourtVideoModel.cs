using Dribbly.Model.Courts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Shared
{
    [Table("CourtVideos")]
    public class CourtVideoModel
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Video")]
        public long VideoId { get; set; }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("Court")]
        public long CourtId { get; set; }

        public virtual CourtModel Court { get; set; }
        public virtual VideoModel Video { get; set; }
    }
}
