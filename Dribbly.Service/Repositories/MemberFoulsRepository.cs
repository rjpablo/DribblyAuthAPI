using Dribbly.Model;
using Dribbly.Model.Entities;
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

        public MemberFoulsRepository(IAuthContext context) : base(context.MemberFouls) {
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

            foul.AdditionalData = JsonConvert.SerializeObject(new { foulName = foul.Foul.Name });

            // prevent EF from re-adding these objects
            foul.Foul = null;
            foul.PerformedBy = null;
            foul.Game = null;

            _context.MemberFouls.Add(foul);
            await _context.SaveChangesAsync();

            var fouls = await _context.MemberFouls
                .Where(f => f.PerformedById == foul.PerformedById && f.GameId == foul.GameId)
                .ToListAsync();

            var result = new UpsertFoulResultModel
            {
                TotalPersonalFouls = fouls.Count,
                TotalTechnicalFouls = fouls.Count(f => f.IsTechnical)
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