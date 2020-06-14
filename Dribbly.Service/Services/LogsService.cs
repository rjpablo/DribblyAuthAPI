using Dribbly.Core.Utilities;
using Dribbly.Service.Models;
using Dribbly.Service.Models.Logs;
using System.Threading.Tasks;

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
    }
}