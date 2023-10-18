using Dribbly.Model.Entities;
using System.Collections.Generic;

namespace Dribbly.Model.Account
{
    public class AccountViewerModel
    {
        public PlayerModel Account { get; set; }
        public PlayerStatsModel Stats { get; set; }
        public IEnumerable<AccountFlag> Flags { get; set; }
    }
}