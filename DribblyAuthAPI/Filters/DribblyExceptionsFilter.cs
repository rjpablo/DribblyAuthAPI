using Dribbly.Core.Exceptions;
using Dribbly.Service.Services;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace DribblyAuthAPI.Filters
{
    public class DribblyExceptionsFilter : ExceptionFilterAttribute
    {
        private ILogsService _logger;

        public DribblyExceptionsFilter(ILogsService logger)
        {
            _logger = logger;
        }

        public async override Task OnExceptionAsync(HttpActionExecutedContext context, CancellationToken cancellationToken)
        {
            long logId = await _logger.LogExceptionAsync(context.Exception);
            if (context.Exception is DribblyException)
            {
                var ex = (DribblyException)context.Exception;
                ex.Data["logId"] = logId;
                context.Response = context.Request
                    .CreateErrorResponse(ex.StatusCode, ex.Message, ex);
                var formatter = new JsonMediaTypeFormatter();
                formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                context.Response.Content = new ObjectContent(typeof(ExceptionResponseContent), new ExceptionResponseContent(ex), formatter);
            }
        }
    }
}