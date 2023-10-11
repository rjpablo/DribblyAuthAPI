using Dribbly.Core.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Courts
{
    [Table("CourtPhotos")]
    public class CourtPhotoModel
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Photo")]
        public long PhotoId { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Court")]
        public long CourtId { get; set; }

        [JsonIgnore]
        public virtual CourtModel Court { get; set; }
        public virtual MultimediaModel Photo { get; set; }
    }
}