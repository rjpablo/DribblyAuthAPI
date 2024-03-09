using Dribbly.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dribbly.Model.Posts
{
    public class GetPostsInputModel
    {
        [Required]
        public EntityTypeEnum PostedOnType { get; set; }

        public long? PostedOnId { get; set; }

        public int GetCount { get; set; }

        public DateTime? AddedBefore { get; set; }
    }
}