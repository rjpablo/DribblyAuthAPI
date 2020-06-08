using Dribbly.Core.Utilities;
using DribblyAuthAPI.Models;
using DribblyAuthAPI.Models.Logs;
using System.Threading.Tasks;

namespace DribblyAuthAPI.Services
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