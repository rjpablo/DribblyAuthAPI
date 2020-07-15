using System;
using System.Collections.Generic;
using System.Text;

namespace Dribbly.Core.Utilities
{
    public interface ISecurityUtility
    {
        string GetUserName();
        string GetUserId();
        bool IsCurrentUser(string userId);
    }
}
