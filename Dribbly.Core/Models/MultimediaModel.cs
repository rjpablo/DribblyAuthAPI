using Dribbly.Core.Enums;
using System;

namespace Dribbly.Core.Models
{
    public class MultimediaModel : BaseEntityModel
    {
        public string Url { get; set; }
        public long UploadedById { get; set; }
        public DateTime? DateDeleted { get; set; }
        public MultimediaTypeEnum Type { get; set; }
    }
}