using DribblyAuthAPI.Models.Logs;
using System.Threading.Tasks;

namespace DribblyAuthAPI.Services
{
    public interface ILogsService
    {
        Task LogClientError(ClientLogModel log);
    }
}