namespace Dribbly.Core.Exceptions
{
    /// <summary>
    /// The content of the Data property of the exception received by the client.
    /// </summary>
    public class ExceptionResponseContent
    {
        public string ExceptionMessage { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string FriendlyMessageKey { get; set; }
        public string FriendlyMessage { get; set; }

        public ExceptionResponseContent(DribblyException ex)
        {
            ExceptionMessage = ex.Message;
            Message = ex.Message;
            StackTrace = ex.StackTrace;
            FriendlyMessageKey = ex.FriendlyMessageKey;
            FriendlyMessage = ex.FriendlyMessage;
        }
    }
}
