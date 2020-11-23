using Dribbly.Model.Shared;

namespace Dribbly.Model.Account
{
    public class AccountDetailsModalModel
    {
        public AccountModel Account { get; set; }
        public ChoiceItemModel<long> HomeCourtChoice { get; set; }
    }
}
