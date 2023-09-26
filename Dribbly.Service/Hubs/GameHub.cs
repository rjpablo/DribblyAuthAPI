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

        public void SetBonus(dynamic input)
        {
            Clients.OthersInGroup(input.gameId.ToString()).setBonus(input);
        }

        public void SetNextPossession(dynamic input)
        {
            Clients.OthersInGroup(input.gameId.ToString()).setNextPossession(input);
        }

        public void SetTeamFoulCount(dynamic input)
        {
            Clients.OthersInGroup(input.gameId.ToString()).setTeamFoulCount(input);
        }
    }

    public interface IGameHub
    {

    }
}