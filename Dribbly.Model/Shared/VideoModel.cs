using Dribbly.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.UI.WebControls;

namespace Dribbly.Model.Shared
{
    [Table("Videos")]
    public class VideoModel : MultimediaModel
    {
        [MaxLength(2000)]
        public string Description { get; set; }

        public VideoModel()
        {
            Type = Core.Enums.MultimediaTypeEnum.Video;
        }
    }
}
