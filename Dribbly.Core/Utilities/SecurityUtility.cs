using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace Dribbly.Core.Utilities
{
    public class SecurityUtility : ISecurityUtility
    {
        HttpContextBase _httpContext;
        public SecurityUtility(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }

        public string GetUserName()
        {
            List<Claim> claims = (_httpContext.User as ClaimsPrincipal).Claims.ToList();
            return _httpContext.User?.Identity.Name;
        }

        List<Claim> GetClaims()
        {
            return (_httpContext.User as ClaimsPrincipal)?.Claims.ToList();
        }

        public string GetUserId()
        {
            return GetClaims()?.SingleOrDefault(c => c.Type == "userId")?.Value;
        }

    }
}
