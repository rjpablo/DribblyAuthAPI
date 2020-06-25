using Dribbly.Model.Games;
using System.Collections.Generic;

namespace Dribbly.Service.Services
{
    public interface IGamesService
    {
        IEnumerable<GameModel> GetAll();

        GameModel GetGame(long id);

        GameModel BookGame(GameModel Game);

        void UpdateGame(GameModel Game);
    }
}