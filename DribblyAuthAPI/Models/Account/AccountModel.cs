using DribblyAuthAPI.Enums;
using DribblyAuthAPI.Models.Auth;
using DribblyAuthAPI.Models.Courts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DribblyAuthAPI.Models.Account
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
        public string IdentityUserId { get; set; }

        [NotMapped]
        public string Username { get; set; }

        [NotMapped]
        public virtual string Email { get; set; }

        public double? HeightInches { get; set; }

        public SexEnum? Sex { get; set; }

        [NotMapped]
        public int? Age {
            get{
                if (BirthDate == null) return null;
                DateTime now = DateTime.UtcNow;
                int age = now.Year - BirthDate.Value.Year;
                if(BirthDate?.Date > now.AddYears(-age))
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

        public virtual PhotoModel ProfilePhoto { get; set; }

        public void Merge(ApplicationUser user)
        {
            if (user != null)
            {
                Username = user.UserName;
                Email = user.Email;
            }
        }
    }
}