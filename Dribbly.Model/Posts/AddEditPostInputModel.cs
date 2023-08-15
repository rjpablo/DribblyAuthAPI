﻿using Dribbly.Model.Enums;
using Dribbly.Service.Enums;
using System.ComponentModel.DataAnnotations;

namespace Dribbly.Model.Posts
{
    public class AddEditPostInputModel
    {
        public long Id { get; set; }

        [Required]
        public EntityTypeEnum AddedByType { get; set; }

        public string Content { get; set; }

        [Required]
        public EntityTypeEnum PostedOnType { get; set; }

        [Required]
        public long PostedOnId { get; set; }

        public PostTypeEnum? Type { get; set; }
        public string AdditionalData { get; set; }
    }
}