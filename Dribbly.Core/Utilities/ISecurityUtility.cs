using System;
using System.Collections.Generic;
using System.Text;

namespace Dribbly.Core.Utilities
{
    public interface ISecurityUtility
    {
        string GetUserName();
        long? GetUserId();
        long? GetAccountId();
        bool IsAuthenticated();
        bool IsCurrentUser(long userId);
        bool IsCurrentAccount(long accountId);
    }
}
