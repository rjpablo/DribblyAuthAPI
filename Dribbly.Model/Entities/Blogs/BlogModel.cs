using Dribbly.Core.Enums;
using Dribbly.Core.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities.Blogs
{
    [Table("Blogs")]
    public class BlogModel : BaseEntityModel, IIndexedEntity
    {
        public EntityTypeEnum EntityType => EntityTypeEnum.Blog;

        public string Name { get; set; }

        public string Slug { get; set; }

        public string IconUrl { get; set; }

        public EntityStatusEnum EntityStatus { get; set; }

        public string Description { get; set; }

        [ForeignKey(nameof(AddedBy))]
        public long AddedById { get; set; }

        public AccountModel AddedBy { get; set; }

        public string Content { get; set; }

        public int VisitCount { get; set; }

        public DateTime LastModified { get; set; }
    }
}
