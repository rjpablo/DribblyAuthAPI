using System.ComponentModel.DataAnnotations;

namespace Dribbly.Authentication.Models.Auth
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}