using Dribbly.Authentication.Models.Auth;
using Dribbly.Core.Models;
using Dribbly.Identity.Models;
using Dribbly.Model.Accounts;
using Dribbly.Model.Courts;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Account
{
    /// <summary>
    /// A clone of ApplicationUser class without the sensitive data.
    /// </summary>
    [Table("Accounts")]
    public class AccountModel : BaseEntityModel
    {
        /// <summary>
        /// Serves as a foreign key to the AspNetUsers table.
        /// This field is used as foreign key in other models instead of the inherited Id field.
        /// </summary>
        [ForeignKey("User")]
        public long IdentityUserId { get; set; }
        [NotMapped]
        public string Username { get { return User.UserName; } }
        [NotMapped]
        public virtual string Email { get { return User.Email; } }
        public PlayerPositionEnum? Position { get; set; }
        [ForeignKey(nameof(HomeCourt))]
        public long? HomeCourtId { get; set; }
        public double? HeightInches { get; set; }
        public GenderEnum? Gender { get; set; }
        public EntityStatusEnum Status { get; set; } = EntityStatusEnum.Active;
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
            get { return Status == EntityStatusEnum.Active; }
        }
        [NotMapped]
        public bool IsInactive
        {
            get { return Status == EntityStatusEnum.Inactive; }
        }
        [NotMapped]
        public bool IsDeleted
        {
            get { return Status == EntityStatusEnum.Deleted ; }
        }
        public bool IsPublic { get; set; }

        public virtual ICollection<AccountPhotoModel> Photos { get; set; }
        public virtual PhotoModel ProfilePhoto { get; set; }
        public virtual ICollection<AccountVideoModel> Videos { get; set; }
        public virtual ApplicationUser User { get; set; }
        public CourtModel HomeCourt { get; set; }

        public AccountBasicInfoModel ToBasicInfo()
        {
            return new AccountBasicInfoModel(this);
        }
    }
}