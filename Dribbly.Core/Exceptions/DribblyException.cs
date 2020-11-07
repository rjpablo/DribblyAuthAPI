using System;
using System.Net;

namespace Dribbly.Core.Exceptions
{
    public class DribblyException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string FriendlyMessageKey { get; set; }
        public string FriendlyMessage { get; set; }
        public long? LogId { get; set; }

        public DribblyException(string message, HttpStatusCode statusCode, string friendlyMessageKey = "",
            string friendlyMessage = "") : base(message)
        {
            StatusCode = statusCode;
            FriendlyMessageKey = friendlyMessageKey;
            FriendlyMessage = friendlyMessage;
        }
    }

    #region Derived Classes

    public class DribblyForbiddenException : DribblyException
    {
        public DribblyForbiddenException(string message, string friendlyMessageKey = "",
            string friendlyMessage = "") : base(message, HttpStatusCode.Forbidden, friendlyMessageKey, friendlyMessage)
        {}
    }

    public class DribblyInvalidOperationException : DribblyException
    {
        public DribblyInvalidOperationException(string message, string friendlyMessageKey = "",
            string friendlyMessage = "") : base(message, HttpStatusCode.Conflict, friendlyMessageKey, friendlyMessage)
        {}
    }

    public class DribblyObjectNotFoundException : DribblyException
    {
        public DribblyObjectNotFoundException(string message, string friendlyMessageKey = "",
            string friendlyMessage = "") : base(message, HttpStatusCode.NotFound, friendlyMessageKey, friendlyMessage)
        {}
    }

    #endregion
}
