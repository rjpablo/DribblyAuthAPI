using System.ComponentModel.DataAnnotations;

namespace DribblyAuthAPI.Models.Auth
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
        public string Userid { get; set; }
    }
}