using Dribbly.Model.Games;
using Dribbly.Model.Play;
using Dribbly.Service.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public interface IGamesService
    {
        IEnumerable<GameModel> GetAll();

        Task<GameModel> GetGame(long id);

        Task<AddGameModalModel> GetAddGameModalAsync(long courtId);

        Task AdvancePeriodAsync(long gameId, int period, int remainingTime);

        Task<GameModel> AddGameAsync(AddGameInputModel input);

        Task EndGameAsync(long gameId, long winningTeamId);

        Task UpdateStatusAsync(long gameId, GameStatusEnum toStatus);

        Task<GameModel> UpdateGameAsync(UpdateGameModel Game);

        Task UpdateGameResultAsync(GameResultModel result);

        Task<DTO.GameTeam> GetGameTeamAsync(long gameId, long teamId);

        Task<GameModel> RecordShotAsync(ShotDetailsInputModel input);

        Task UpdateRemainingTimeAsync(UpdateGameTimeRemainingInput input);
    }
}