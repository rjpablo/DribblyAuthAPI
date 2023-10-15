using Dribbly.Core.Enums;
using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Courts;
using Dribbly.Model.Entities;
using Dribbly.Model.Enums;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Posts
{
    // Games table has an additional ID column which is a Foreign Key to the bookings table
    [Table("Posts")]
    public class PostModel : BaseEntityModel, IIndexedEntity
    {
        #region MappedColumns
        
        /// <summary>
        /// The ID of the entity that created this post
        /// </summary>
        [ForeignKey(nameof(AddedBy))]
        public long AddedById { get; set; }

        /// <summary>
        /// The type of the entity that created this post
        /// </summary>
        public EntityTypeEnum AddedByType { get; set; }

        public string Content { get; set; }

        public string EmbedCode { get; set; }

        /// <summary>
        /// On which type of entity was this post posted
        /// </summary>
        public EntityTypeEnum PostedOnType { get; set; }

        /// <summary>
        /// The ID of the entity on which this post was posted
        /// </summary>
        public long PostedOnId { get; set; }

        public EntityStatusEnum EntityStatus { get; set; }

        public PostTypeEnum? Type { get; set; }

        [NotMapped]
        public EntityTypeEnum EntityType { get; } = EntityTypeEnum.Post;
        public string AdditionalData { get; set; }
        public ICollection<PostFile> Files { get; set; }

        #endregion

        #region Other Properties
        [NotMapped]
        public string Name { get; set; } = "";

        [NotMapped]
        public string IconUrl { get; set; } = "";

        [NotMapped]
        public string Description { get { return Content; } }

        public PlayerModel AddedBy { get; set; }

        #endregion


    }
}