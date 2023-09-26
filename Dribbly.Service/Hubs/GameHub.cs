using Dribbly.Model.Games;
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

        public void UpdateClock(UpdateGameTimeRemainingInput input)
        {
            Clients.OthersInGroup(input.GameId.ToString()).UpdateClock(input);
        }

        public void UpdatePeriod(dynamic input)
        {
            Clients.OthersInGroup(input.gameId.ToString()).updatePeriod(input);
        }

        public void SetTol(dynamic input)
        {
            Clients.OthersInGroup(input.gameId.ToString()).setTol(input);
        }
    }

    public interface IGameHub
    {

    }
}