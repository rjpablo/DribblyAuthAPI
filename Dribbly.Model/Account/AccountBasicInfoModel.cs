using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Account
{
    [NotMapped]
    public class AccountBasicInfoModel: EntityBasicInfoModel
    {
        public long IdentityUserId { get; set; }
        public GenderEnum? Gender { get; set; }
        public bool IsPublic { get; set; }

        public AccountBasicInfoModel()
        {

        }

        public AccountBasicInfoModel(AccountModel account)
        {
            IdentityUserId = account.IdentityUserId;
            Id = account.Id;
            Name = account.User != null ? account.User.UserName : account.Username;
            Gender = account.Gender;
            Photo = account.ProfilePhoto;
            Status = account.Status;
            IsPublic = account.IsPublic;
        }
    }
}