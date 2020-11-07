using Dribbly.Model.Logs;
using System;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public interface ILogsService
    {
        Task LogClientError(ClientLogModel log);
        Task<long> LogExceptionAsync(Exception ex);
    }
}