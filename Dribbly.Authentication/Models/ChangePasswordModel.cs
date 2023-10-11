using System.ComponentModel.DataAnnotations;

namespace Dribbly.Authentication.Models.Auth
{
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        public long Userid { get; set; }
    }
}