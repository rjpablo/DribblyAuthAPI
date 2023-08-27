using Dribbly.Core.Models;
using Dribbly.Identity.Models;
using Dribbly.Model.Accounts;
using Dribbly.Model.Courts;
using Dribbly.Model.Entities;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Account
{
    /// <summary>
    /// A clone of ApplicationUser class without the sensitive data.
    /// </summary>
    [Table("Players")]
    public class PlayerModel : AccountModel, IIndexedEntity
    {
        /// <summary>
        /// Serves as a foreign key to the AspNetUsers table.
        /// This field is used as foreign key in other models instead of the inherited Id field.
        /// </summary>
        [ForeignKey("User")]
        public long IdentityUserId { get; set; }
        [NotMapped]
        public virtual string Email { get { return User?.Email; } }
        public PlayerPositionEnum? Position { get; set; }
        [ForeignKey(nameof(HomeCourt))]
        public long? HomeCourtId { get; set; }
        public double? HeightInches { get; set; }
        public double? Rating { get; set; }

        public virtual ICollection<AccountPhotoModel> Photos { get; set; }
        public virtual ICollection<AccountVideoModel> Videos { get; set; }
        public virtual ICollection<AccountHighlightModel> Highlights { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }
        public CourtModel HomeCourt { get; set; }

        public AccountBasicInfoModel ToBasicInfo()
        {
            return new AccountBasicInfoModel(this);
        }
    }
}