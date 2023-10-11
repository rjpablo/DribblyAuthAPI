using Dribbly.Core.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Logs
{
    [Table("ClientLogs")]
    public class ClientLogModel : BaseEntityModel
    {
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public string Stack { get; set; }
        public string Url { get; set; }
        public int ErrorCode { get; set; }
        public int LineNo { get; set; }
        public int Column { get; set; }
        public string LoggedBy { get; set; }
        public string Browser { get; set; }
        public string Os { get; set; }
    }
}