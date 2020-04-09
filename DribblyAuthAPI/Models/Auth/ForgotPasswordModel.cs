using System.ComponentModel.DataAnnotations;

namespace DribblyAuthAPI.Models.Auth
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}