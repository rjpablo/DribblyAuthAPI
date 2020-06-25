using Dribbly.Model.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Courts
{
    [Table("Courts")]
    public class CourtModel : BaseEntityModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public new long Id { get; set; }

        public string OwnerId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public decimal RatePerHour { get; set; }

        public decimal? Rating { get; set; }

        public string PrimaryPhotoUrl { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public bool IsPublic { get; set; }

        public string AdditionalInfo { get; set; }

        public virtual ICollection<CourtPhotoModel> Photos { get; set; }

        [NotMapped]
        public virtual AccountBasicInfoModel Owner { get; set; }

    }
}