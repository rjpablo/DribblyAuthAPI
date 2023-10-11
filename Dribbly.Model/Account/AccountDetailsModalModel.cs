using Dribbly.Core.Models;

namespace Dribbly.Model.Account
{
    public class AccountDetailsModalModel
    {
        public PlayerModel Account { get; set; }
        public ChoiceItemModel<long> HomeCourtChoice { get; set; }
    }
}
