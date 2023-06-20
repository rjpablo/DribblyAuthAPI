using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Leagues;
using Dribbly.Model.Seasons;
using Dribbly.Service.Repositories;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public class SeasonsService : BaseEntityService<SeasonModel>, ISeasonsService
    {
        IAuthContext _context;
        ISecurityUtility _securityUtility;
        private readonly IIndexedEntitysRepository _indexedEntitysRepository;
        private readonly ISeasonsRepository _seasonsRepository;

        public SeasonsService(IAuthContext context,
            ISecurityUtility securityUtility,
            IIndexedEntitysRepository indexedEntitysRepository) : base(context.Seasons)
        {
            _context = context;
            _securityUtility = securityUtility;
            _indexedEntitysRepository = indexedEntitysRepository;
            _seasonsRepository = new SeasonsRepository(context);
        }

        public async Task<SeasonModel> AddSeasonAsync(SeasonModel season)
        {
            season.AddedById = _securityUtility.GetUserId().Value;
            _seasonsRepository.Add(season);
            await _context.SaveChangesAsync();
            // TODO: log activity
            return season;
        }
    }

    public interface ISeasonsService
    {
        Task<SeasonModel> AddSeasonAsync(SeasonModel season);
    }
}