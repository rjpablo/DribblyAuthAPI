using Dribbly.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dribbly.Core.Models
{
    public class MultimediaModel : BaseEntityModel
    {
        [MaxLength(100)]
        public string Title { get; set; }
        public string Url { get; set; }
        public long UploadedById { get; set; }
        public DateTime? DateDeleted { get; set; }
        public MultimediaTypeEnum Type { get; set; }
        public long Size { get; set; }
    }
}