using System;
using System.Security.Claims;
using System.Web;

namespace Dribbly.Authentication.Services
{
    public class AuthenticationService
    {
        public static bool HasPermission(Enum permission)
        {
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return (HttpContext.Current.User as ClaimsPrincipal).HasClaim("Permission", (Convert.ToInt32(permission)).ToString());
            }

            return false;
            
        }
    }
}
