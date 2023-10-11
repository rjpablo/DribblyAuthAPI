using Dribbly.Model.Games;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.UserActivities
{
    [Table("UserGameActivities")]
    public class UserGameActivityModel : UserActivityModel
    {
        [ForeignKey(nameof(Game))]
        public long? GameId { get; set; }
        public GameModel Game { get; set; }

    }
}
