using System.ComponentModel.DataAnnotations;

namespace Dribbly.Service.Models.Auth
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}