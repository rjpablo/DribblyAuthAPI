using Dribbly.Chat.Models.ViewModels;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace Dribbly.Chat.Hubs
{
    public class ChatHub : Hub, IChatHub
    {
        public ChatHub():base()
        {
        }
        public string GetConnectionId() => Context.ConnectionId;

        public void UnviewedCountChanged(string groupName, object data)
        {
            Clients.Group(groupName).unviewedCountChanged(data);
        }

        public void ReceiveMessage(string groupName, MessageViewModel message)
        {
            Clients.Group(groupName).ReceiveMessage(message);
        }
        public async Task JoinGroup(string connectionId, string groupName)
        {
            await Groups.Add(connectionId, groupName);
        }
    }

    public interface IChatHub
    {
        string GetConnectionId();
        void UnviewedCountChanged(string groupName, object data);
        void ReceiveMessage(string groupName, MessageViewModel message);
        Task JoinGroup(string connectionId, string groupName);
    }
}
