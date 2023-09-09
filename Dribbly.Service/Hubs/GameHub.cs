using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace Dribbly.Service.Hubs
{
    public class GameHub : Hub, IGameHub
    {
        public async Task JoinGroup(string connectionId, string groupName)
        {
            await Groups.Add(connectionId, groupName);
        }
    }

    public interface IGameHub
    {

    }
}