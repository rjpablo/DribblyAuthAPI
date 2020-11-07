using Dribbly.Core.Models;

namespace Dribbly.Model.Logs
{
    public class ExceptionLog: BaseEntityModel
    {
        public string Message { get; set; }
        public string RequestUrl { get; set; }
        public string RequestData { get; set; }
        public string StackTrace { get; set; }
        public string LoggedBy { get; set; }
    }
}
