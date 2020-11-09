using Dribbly.Core.Models;
using Dribbly.Service.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.UserActivities
{
    [Table("UserActivities")]
    public class UserActivityModel : BaseEntityModel
    {
        public UserActivityTypeEnum Type { get; set; }
        public string UserId { get; set; }
    }
}
