using Dribbly.Service.Enums;
using Dribbly.Service.Models.Courts;

namespace Dribbly.Service.Models.Account
{
    public class AccountBasicInfoModel
    {
        public string IdentityUserId { get; set; }

        public string Username { get; set; }

        public SexEnum? Sex { get; set; }

        public virtual PhotoModel ProfilePhoto { get; set; }

        public AccountBasicInfoModel(AccountModel account)
        {
            IdentityUserId = account.IdentityUserId;
            Username = account.Username;
            Sex = account.Sex;
            ProfilePhoto = account.ProfilePhoto;
        }
    }
}