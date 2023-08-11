using Dribbly.Model.Entities;
using Dribbly.Model.Shared;

namespace Dribbly.Model.Account
{
    public class AccountViewerModel
    {
        public AccountModel Account { get; set; }
        public PlayerStatsModel Stats { get; set; }
    }
}