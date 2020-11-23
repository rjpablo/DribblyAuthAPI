using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Courts
{
    [Table("Courts")]
    public class CourtModel : BaseEntityModel, IIndexedEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public new long Id { get; set; }

        public long OwnerId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public decimal RatePerHour { get; set; }

        public double Rating { get; set; }

        [ForeignKey("PrimaryPhoto")]
        public long? PrimaryPhotoId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public bool IsFreeToPlay { get; set; }

        [ForeignKey("Contact")]
        public long? ContactId { get; set; }

        public EntityStatusEnum Status { get; set; }

        [NotMapped]
        public bool IsDeleted { get { return Status == EntityStatusEnum.Deleted; } }

        [NotMapped]
        public bool IsActive { get { return Status == EntityStatusEnum.Active; } }

        [NotMapped]
        public bool IsInactive { get { return Status == EntityStatusEnum.Inactive; } }

        public string AdditionalInfo { get; set; }

        public virtual ICollection<CourtPhotoModel> Photos { get; set; }

        public virtual ICollection<CourtVideoModel> Videos { get; set; }

        public virtual ContactModel Contact { get; set; }

        public virtual PhotoModel PrimaryPhoto { get; set; }

        [NotMapped]
        public string IconUrl { get { return PrimaryPhoto?.Url; } }

        [NotMapped]
        public EntityTypeEnum EntityType { get; } = EntityTypeEnum.Court;

        [NotMapped]
        public string Description { get { return AdditionalInfo; } }

        [NotMapped]
        public object this[string propertyName]
        {
            set
            {
                typeof(CourtModel).GetProperty(propertyName).SetValue(this, value, null);
            }
        }
    }
}