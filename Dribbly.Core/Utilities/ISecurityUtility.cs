using System;
using System.Collections.Generic;
using System.Text;

namespace Dribbly.Core.Utilities
{
    public interface ISecurityUtility
    {
        string GetUserName();
        long? GetUserId();
        bool IsAuthenticated();
        bool IsCurrentUser(long userId);
    }
}
