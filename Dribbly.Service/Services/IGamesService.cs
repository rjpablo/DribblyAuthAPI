using Dribbly.Model.Games;
using Dribbly.Service.Enums;
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

        Task UpdateStatusAsync(long gameId, GameStatusEnum toStatus);

        Task UpdateGameAsync(UpdateGameModel Game);

        Task UpdateGameResultAsync(GameResultModel result);

        Task<DTO.GameTeam> GetGameTeamAsync(long gameId, long teamId);

        Task<GameModel> RecordShotAsync(ShotModel shot);
    }
}