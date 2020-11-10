using Dribbly.Model.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.UserActivities
{
    [Table("UserContactActivities")]
    public class UserContactActivityModel : UserActivityModel
    {
        [ForeignKey(nameof(Contact))]
        public long? ContactId { get; set; }
        public string ContactNo { get; set; }

        public ContactModel Contact { get; set; }

    }
}
