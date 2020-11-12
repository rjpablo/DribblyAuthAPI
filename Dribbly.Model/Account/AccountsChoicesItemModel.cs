using Dribbly.Model.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Account
{
    [NotMapped]
    public class AccountsChoicesItemModel : ChoiceItemModel<long>
    {
        public AccountsChoicesItemModel() { }

        public AccountsChoicesItemModel(AccountModel account)
        {
            Text = account.Username;
            Value = account.IdentityUserId;
            IconUrl = account.ProfilePhoto?.Url;
        }
    }
}
