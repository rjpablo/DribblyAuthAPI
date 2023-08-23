using System;

namespace Dribbly.Core.Models
{
    public class PhotoModel : BaseEntityModel
    {
        public string Url { get; set; }
        public long UploadedById { get; set; }
        public DateTime? DateDeleted { get; set; }
    }
}