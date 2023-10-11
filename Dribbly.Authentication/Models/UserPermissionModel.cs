using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Authentication.Models
{
    [Table("UserPermissions")]
    public class UserPermissionModel
    {
        [Key]
        [Column(Order = 1)]
        public string UserId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int PermissionId { get; set; }
    }
}
