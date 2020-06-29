using System.Linq;

namespace System.Security.Claims
{
    public static class ClaimsExtensions
    {
        public static bool HasClaim(this ClaimsPrincipal principal, string claim)
        {
            return principal.Claims.ToList().FirstOrDefault(x => x.Type.Equals(claim, StringComparison.OrdinalIgnoreCase)) != null;
        }
    }
}
