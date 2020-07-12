﻿using Dribbly.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.UI.WebControls;

namespace Dribbly.Model.Shared
{
    [Table("Videos")]
    public class VideoModel : BaseEntityModel
    {
        public string Src { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        public string AddedBy { get; set; }

        public long Size { get; set; }

        public string Type { get; set; }
    }
}
