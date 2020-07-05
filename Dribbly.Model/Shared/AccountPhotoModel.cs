using Dribbly.Model.Account;
using Dribbly.Model.Courts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Accounts
{
    [Table("AccountPhotos")]
    public class AccountPhotoModel
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Photo")]
        public long PhotoId { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Account")]
        public long AccountId { get; set; }

        public virtual AccountModel Account { get; set; }
        public virtual PhotoModel Photo { get; set; }
    }
}