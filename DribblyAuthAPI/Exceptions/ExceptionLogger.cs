using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Service.Services;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace DribblyAuthAPI.Exceptions
{
    public class DribblyExceptionLogger : IExceptionLogger
    {
        string loggedByKey = "loggedBy";
        public virtual Task LogAsync(ExceptionLoggerContext context,
                                     CancellationToken cancellationToken)
        {
            if (!ShouldLog(context))
            {
                return Task.FromResult(0);
            }

            return LogAsyncCore(context, cancellationToken);
        }

        public virtual Task LogAsyncCore(ExceptionLoggerContext context,
                                         CancellationToken cancellationToken)
        {
            return LogCore(context);
        }

        public virtual async Task LogCore(ExceptionLoggerContext context)
        {
            ILogsService _logger = new LogsService(new AuthContext(), new SecurityUtility(new HttpContextWrapper(HttpContext.Current)));
            var logId = await _logger.LogExceptionAsync(context.ExceptionContext.Exception);
            context.ExceptionContext.Exception.Data["logId"] = logId;
        }

        /// <summary>
        /// Returns true if the exception has NOT been logged
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual bool ShouldLog(ExceptionLoggerContext context)
        {
            IDictionary exceptionData = context.ExceptionContext.Exception.Data;

            if (!exceptionData.Contains(loggedByKey))
            {
                exceptionData.Add(loggedByKey, new List<object>());
            }

            ICollection<object> loggedBy = ((ICollection<object>)exceptionData[loggedByKey]);

            if (!loggedBy.Contains(this))
            {
                loggedBy.Add(this);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}