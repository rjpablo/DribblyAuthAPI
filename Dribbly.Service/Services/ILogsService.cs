using Dribbly.Model.Logs;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public interface ILogsService
    {
        Task LogClientError(ClientLogModel log);
    }
}