using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace Dribbly.Authentication.Attributes
{
    public class DribblyAuthorize: AuthorizeAttribute
    {
        /// <summary>
        /// Multiple values will be treated as OR
        /// </summary>
        public object[] Permissions { get; set; }
        private string PermissionErrorMessage = "User does not have the required permission/s.";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissions">A collection of Permission Enums</param>
        public DribblyAuthorize(params object[] permissions)
        {
            Permissions = permissions;
        }

        public override void OnAuthorization(
           System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);

            if(actionContext.Response == null) // means user is authenticated
            {
                if(Permissions.Length == 1)
                {
                    if (!(HttpContext.Current.User as ClaimsPrincipal).HasClaim("Permission", (Convert.ToInt32(Permissions[0])).ToString()))
                    {
                        actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, PermissionErrorMessage);
                    }
                }
                else if (Permissions.Length > 1)
                {
                    bool hasPermission = false;
                    foreach (int permission in Permissions)
                    {
                        if ((HttpContext.Current.User as ClaimsPrincipal).HasClaim("Permission", permission.ToString()))
                        {
                            hasPermission = true;
                            break;
                        }
                    }
                    if (!hasPermission)
                    {
                        actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, PermissionErrorMessage);
                    }
                }
            }
        }
    }
}
