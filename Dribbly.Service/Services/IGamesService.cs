using Dribbly.Model.Games;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public interface IGamesService
    {
        IEnumerable<GameModel> GetAll();

        Task<GameModel> GetGame(long id);

        Task<GameModel> BookGameAsync(GameModel Game);

        Task UpdateGameAsync(GameModel Game);
    }
}