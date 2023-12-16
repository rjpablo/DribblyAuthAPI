using Dribbly.Core.Enums;
using Dribbly.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dribbly.Model.Comments
{
    public class GetCommentsInputModel: PagedGetInputModel
    {
        [Required]
        public EntityTypeEnum CommentedOnType { get; set; }

        public long? CommentedOnId { get; set; }

        public int GetCount { get; set; }

        public long? CeilingPostId { get; set; }
        public DateTime? AfterDate { get; set; }
    }
}
