using Dribbly.Model.Courts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.UserActivities
{
    [Table("AccountPhotoActivities")]
    public class AccountPhotoActivityModel : UserAccountActivityModel
    {
        [ForeignKey(nameof(Photo))]
        public long PhotoId { get; set; }

        public PhotoModel Photo { get; set; }

    }
}
