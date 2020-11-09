using Dribbly.Model.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.UserActivities
{
    [Table("AccountVideoActivities")]
    public class AccountVideoActivityModel : UserAccountActivityModel
    {
        [ForeignKey(nameof(Video))]
        public long VideoId { get; set; }

        public VideoModel Video { get; set; }

    }
}
