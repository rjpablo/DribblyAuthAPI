using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Dribbly.Service.Hubs
{
    public class NotificationsHub : Hub
    {
        public async Task JoinGroup(string connectionId, string groupName)
        {
            await Groups.Add(connectionId, groupName);
        }
    }

    public interface INotificationsHub
    {

    }
}