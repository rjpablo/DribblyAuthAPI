using Dribbly.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Teams
{
    [Table("TeamPhotos")]
    public class TeamPhotoModel
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Photo")]
        public long PhotoId { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Team")]
        public long TeamId { get; set; }

        public virtual TeamModel Team { get; set; }
        public virtual PhotoModel Photo { get; set; }
    }
}