using Dribbly.Core.Enums;
using Dribbly.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Core.Models
{
    /// <summary>
    /// A clone of ApplicationUser class without the sensitive data.
    /// </summary>
    [Table("Accounts")]
    public class AccountModel : BaseEntityModel, IIndexedEntity
    {
        [MaxLength(30)]
        public string FirstName { get; set; }
        [MaxLength(30)]
        public string LastName { get; set; }
        public GenderEnum? Gender { get; set; }
        public EntityStatusEnum EntityStatus { get; set; } = EntityStatusEnum.Active;
        [NotMapped]
        public int? Age
        {
            get
            {
                if (BirthDate == null) return null;
                DateTime now = DateTime.UtcNow;
                int age = now.Year - BirthDate.Value.Year;
                if (BirthDate?.Date > now.AddYears(-age))
                {
                    age--;
                }
                return age;
            }
        }
        public string ContactNo { get; set; }
        public DateTime? BirthDate { get; set; }
        [ForeignKey("ProfilePhoto")]
        public long? ProfilePhotoId { get; set; }
        [NotMapped]
        public bool IsActive
        {
            get { return EntityStatus == EntityStatusEnum.Active; }
        }
        [NotMapped]
        public bool IsInactive
        {
            get { return EntityStatus == EntityStatusEnum.Inactive; }
        }
        [NotMapped]
        public bool IsDeleted
        {
            get { return EntityStatus == EntityStatusEnum.Deleted; }
        }
        public bool IsPublic { get; set; } = true;
        public virtual MultimediaModel ProfilePhoto { get; set; }

        #region For IndexedEntity
        [NotMapped]
        public string IconUrl { get { return ProfilePhoto?.Url; } }
        [NotMapped]
        public EntityTypeEnum EntityType { get; } = EntityTypeEnum.Account;
        [NotMapped]
        public string Name { get { return FirstName + " " + LastName; } }
        [NotMapped]
        public string Description { get; }
        #endregion
    }
}