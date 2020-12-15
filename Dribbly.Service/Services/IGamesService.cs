using Dribbly.Model.Games;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public interface IGamesService
    {
        IEnumerable<GameModel> GetAll();

        Task<GameModel> GetGame(long id);

        Task<AddGameModalModel> GetAddGameModalAsync(long courtId);

        Task<GameModel> AddGameAsync(AddGameInputModel input);

        Task UpdateGameAsync(GameModel Game);
    }
}