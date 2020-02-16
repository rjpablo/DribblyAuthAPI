﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DribblyAuthAPI.Models.Courts
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

        public virtual CourtModel Court { get; set; }
        public virtual PhotoModel Photo { get; set; }
    }
}