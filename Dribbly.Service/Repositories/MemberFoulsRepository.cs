using Dribbly.Model;
using Dribbly.Model.Entities;
using Dribbly.Model.Enums;
using Dribbly.Model.Fouls;
using Dribbly.Model.Games;
using Dribbly.Model.Teams;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
{
    public class MemberFoulsRepository : BaseRepository<MemberFoulModel>, IMemberFoulsRepository
    {
        private readonly IAuthContext _context;

        public MemberFoulsRepository(IAuthContext context) : base(context.MemberFouls)
        {
            _context = context;
        }

        public IEnumerable<MemberFoulModel> GetAll()
        {
            return _dbSet;
        }

        public async Task<UpsertFoulResultModel> UpsertFoul(MemberFoulModel foul)
        {
            if (foul.Id == 0) // inserting
            {
                foul.DateAdded = DateTime.UtcNow;
            }

            foul.AdditionalData = JsonConvert.SerializeObject(new { foulName = foul.Foul.Name, foulId = foul.FoulId });
            foul.IsFlagrant = foul.Foul.IsFlagrant;

            // prevent EF from re-adding these objects
            _context.SetEntityState(foul.Foul, EntityState.Unchanged);
            _context.SetEntityState(foul.PerformedBy, EntityState.Unchanged);
            _context.SetEntityState(foul.Game, EntityState.Unchanged);

            _context.MemberFouls.Add(foul);
            await _context.SaveChangesAsync();

            var fouls = await _context.MemberFouls
                .Where(f => f.PerformedById == foul.PerformedById && f.GameId == foul.GameId)
                .ToListAsync();

            // TODO: add game null check
            var gamePlayer = _context.GamePlayers.SingleOrDefault(g => g.TeamMembership.Account.Id == foul.PerformedById
            && g.GameTeam.TeamId == foul.TeamId && g.GameId == foul.GameId);
            var game = _context.Games.SingleOrDefault(g => g.Id == foul.GameId);
            // TODO: add gameplyer null check
            gamePlayer.Fouls = fouls.Count;
            gamePlayer.HasFouledOut = fouls.Count >= game.PersonalFoulLimit;
            gamePlayer.EjectionStatus = foul.Foul.Name == "Flagrant 2" ? EjectionStatusEnum.EjectedDueToFlagrantFoul2 :
                fouls.Count(f => f.IsFlagrant) >= 2 ? EjectionStatusEnum.EjectedDueNumberOfFlagrantFouls :
                EjectionStatusEnum.NotEjected;
            if (foul.IsOffensive)
            {
                gamePlayer.Turnovers++;
            }

            var gameTeam = _context.GameTeams.SingleOrDefault(g => g.GameId == foul.GameId && g.TeamId == foul.TeamId);
            // TODO: add validation
            gameTeam.TeamFoulCount++;
            if (foul.IsOffensive)
            {
                gameTeam.Turnovers++;
            }

            await _context.SaveChangesAsync();

            var result = new UpsertFoulResultModel
            {
                TotalPersonalFouls = gamePlayer.Fouls,
                TotalTechnicalFouls = fouls.Count(f => f.IsTechnical),
                EjectionStatus = gamePlayer.EjectionStatus,
                HasFouledOut = gamePlayer.HasFouledOut,
                TeamFoulCount = gameTeam.TeamFoulCount
            };
            return result;
        }
    }
    public interface IMemberFoulsRepository : IBaseRepository<MemberFoulModel>
    {
        IEnumerable<MemberFoulModel> GetAll();
        Task<UpsertFoulResultModel> UpsertFoul(MemberFoulModel foul);
    }
}