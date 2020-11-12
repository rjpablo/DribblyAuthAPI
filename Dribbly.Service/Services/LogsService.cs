using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Logs;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Dribbly.Service.Services
{
    public class LogsService : BaseEntityService<ClientLogModel>, ILogsService
    {
        IAuthContext _context;
        ISecurityUtility _securityUtility;
        public LogsService(IAuthContext context,
            ISecurityUtility securityUtility) : base(context.ErrorLogs)
        {
            _context = context;
            _securityUtility = securityUtility;
        }

        public async Task LogClientError(ClientLogModel log)
        {
            Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<long> LogExceptionAsync(Exception ex)
        {
            var req = HttpContext.Current.Request;
            var log = new ExceptionLog
            {
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                RequestUrl = req.RawUrl,
                RequestData = TryGetRequestData(req),
                LoggedBy = GetUserId(), //_securityUtility.GetUserId(),
                DateAdded = DateTime.UtcNow
            };
            
            _context.ExceptionLogs.Add(log);
            await _context.SaveChangesAsync();
            return log.Id;
        }

        private long? GetUserId()
        {
            var stringUserId = ClaimsPrincipal.Current.Claims.ToList()
                .SingleOrDefault(c => c.Type == "userId")?.Value;
            if (string.IsNullOrEmpty(stringUserId))
            {
                return null;
            }

            return long.Parse(stringUserId);
        }

        private string TryGetRequestData(HttpRequest request)
        {
            try
            {
                using (StreamReader stream = new StreamReader(request.InputStream))
                {
                    return stream.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return "(failed to read)";
            }
        }
    }
}