﻿using Dribbly.Core.Models;
using Dribbly.Model.Courts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.UserActivities
{
    [Table("CourtPhotoActivities")]
    public class CourtPhotoActivityModel : UserActivityModel
    {
        [ForeignKey(nameof(Photo))]
        public long PhotoId { get; set; }
        [ForeignKey(nameof(Court))]
        public long CourtId { get; set; }

        public MultimediaModel Photo { get; set; }
        public CourtModel Court { get; set; }

    }
}
