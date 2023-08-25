using Dribbly.Model.Account;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.UserActivities
{
    [Table("UserAccountActivities")]
    public class UserAccountActivityModel : UserActivityModel
    {
        [ForeignKey(nameof(Account))]
        public long AccountId { get; set; }

        public PlayerModel Account { get; set; }

    }
}
