using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DribblyAuthAPI.Models.Courts
{
    [Table("Courts")]
    public class CourtModel : BaseModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public long Id { get; set; }

        public long OwnerId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string Address { get; set; }

        public decimal? RatePerHour { get; set; }

        public decimal? Rating { get; set; }

        public string PrimaryPhotoUrl { get; set; }
    }
}