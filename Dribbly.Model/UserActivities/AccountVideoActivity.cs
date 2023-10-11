using Dribbly.Core.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.UserActivities
{
    [Table("AccountPhotoActivities")]
    public class AccountPhotoActivityModel : UserAccountActivityModel
    {
        [ForeignKey(nameof(Photo))]
        public long PhotoId { get; set; }

        public MultimediaModel Photo { get; set; }

    }
}
