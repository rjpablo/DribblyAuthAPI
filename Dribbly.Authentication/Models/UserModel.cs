using System.ComponentModel.DataAnnotations;

namespace Dribbly.Authentication.Models
{
    public class UserModel
    {
        [Required]
        [StringLength(30, ErrorMessage = "The {0} must not exceed {1} characters.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "The {0} must not exceed {1} characters.")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]{5,20}$", ErrorMessage = "Username must 5-20 alphanumeric characters")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }
}