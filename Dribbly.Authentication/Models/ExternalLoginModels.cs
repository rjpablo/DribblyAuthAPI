using System.ComponentModel.DataAnnotations;

namespace Dribbly.Authentication.Models
{
    public class ExternalLoginViewModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string State { get; set; }
    }

    public class RegisterExternalBindingModel
    {
        [Required, RegularExpression(@"^[a-zA-Z0-9]{5,20}$", ErrorMessage = "Username must 5-20 alphanumeric characters")]
        public string UserName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Provider { get; set; }

        [Required]
        public string UserId { get; set; }

        public string Picture { get; set; }

        [Required]
        public string ExternalAccessToken { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

    }

    public class ParsedExternalAccessToken
    {
        public string user_id { get; set; }
        public string app_id { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string email { get; set; }
        public string picture { get; set; }
    }
}