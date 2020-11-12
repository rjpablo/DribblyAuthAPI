using Dribbly.Model.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Account
{
    [NotMapped]
    public class AccountsChoicesItemModel : ChoiceItemModel<string>
    {
        public AccountsChoicesItemModel() { }

        public AccountsChoicesItemModel(AccountModel account)
        {
            Text = account.Username;
            Value = account.IdentityUserId.ToString();
            IconUrl = account.ProfilePhoto.Url;
        }
    }
}
