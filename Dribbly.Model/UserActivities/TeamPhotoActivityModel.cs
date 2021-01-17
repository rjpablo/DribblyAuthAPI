﻿using Dribbly.Model.Courts;
using Dribbly.Model.Teams;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.UserActivities
{
    [Table("TeamPhotoActivities")]
    public class TeamPhotoActivityModel : UserActivityModel
    {
        [ForeignKey(nameof(Photo))]
        public long PhotoId { get; set; }
        [ForeignKey(nameof(Team))]
        public long TeamId { get; set; }

        public TeamModel Team { get; set; }
        public PhotoModel Photo { get; set; }

    }
}
