using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DribblyAuthAPI.Models.Courts
{
    [Table("Photos")]
    public class PhotoModel : BaseModel
    {
        public string Url { get; set; }
        public string UploadedById { get; set; }
    }
}