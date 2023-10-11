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

        public long? GetUserId()
        {
            string stringUserId = GetClaims()?.SingleOrDefault(c => c.Type == "userId")?.Value;
            if (string.IsNullOrEmpty(stringUserId))
            {
                return null;
            }
            return long.Parse(stringUserId);
        }

        public long? GetAccountId()
        {
            string stringAccountId = GetClaims()?.SingleOrDefault(c => c.Type == "accountId")?.Value;
            if (string.IsNullOrEmpty(stringAccountId))
            {
                return null;
            }
            return long.Parse(stringAccountId);
        }

        public bool IsCurrentUser(long userId)
        {
            return userId.Equals(GetUserId());
        }

        public bool IsCurrentAccount(long accountId)
        {
            var currentAccountId = GetAccountId();
            return !currentAccountId.HasValue || accountId.Equals(currentAccountId);
        }

        public bool IsAuthenticated()
        {
            return _httpContext.User.Identity.IsAuthenticated;
        }

    }
}
