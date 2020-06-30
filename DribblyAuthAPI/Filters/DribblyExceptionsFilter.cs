using System;
using System.Net.Http;
using System.Web.Http.Filters;

namespace DribblyAuthAPI.Filters
{
    public class DribblyExceptionsFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is UnauthorizedAccessException)
            {
                context.Response = context.Request
                    .CreateErrorResponse(System.Net.HttpStatusCode.Forbidden, context.Exception.Message, context.Exception);
            }
        }
    }
}