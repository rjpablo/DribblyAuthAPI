using DribblyAuthAPI.Models.Games;
using System.Collections.Generic;

namespace DribblyAuthAPI.Services
{
    public interface IGamesService
    {
        IEnumerable<GameModel> GetAll();

        GameModel GetGame(long id);

        GameModel BookGame(GameModel Game);

        void UpdateGame(GameModel Game);
    }
}