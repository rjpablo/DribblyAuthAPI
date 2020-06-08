using DribblyAuthAPI.Enums;
using DribblyAuthAPI.Models.Courts;

namespace DribblyAuthAPI.Models.Account
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