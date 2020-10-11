using Dribbly.Service.Enums;
using Dribbly.Model.Courts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Account
{
    [NotMapped]
    public class AccountBasicInfoModel
    {
        public string IdentityUserId { get; set; }
        public string Username { get; set; }
        public GenderEnum? Gender { get; set; }
        public virtual PhotoModel ProfilePhoto { get; set; }
        public AccountStatusEnum Status { get; set; }
        public bool IsPublic { get; set; }

        public AccountBasicInfoModel()
        {

        }

        public AccountBasicInfoModel(AccountModel account)
        {
            IdentityUserId = account.IdentityUserId;
            Username = account.User != null ? account.User.UserName : account.Username;
            Gender = account.Gender;
            ProfilePhoto = account.ProfilePhoto;
            Status = account.Status;
            IsPublic = account.IsPublic;
        }
    }
}