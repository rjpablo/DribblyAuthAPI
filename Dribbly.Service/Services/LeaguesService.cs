using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Leagues;
using Dribbly.Service.Repositories;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public class LeaguesService : BaseEntityService<LeagueModel>, ILeaguesService
    {
        IAuthContext _context;
        ISecurityUtility _securityUtility;
        private readonly IIndexedEntitysRepository _indexedEntitysRepository;
        private readonly ILeaguesRepository _leaguesRepository;

        public LeaguesService(IAuthContext context,
            ISecurityUtility securityUtility,
            IIndexedEntitysRepository indexedEntitysRepository) : base(context.Leagues)
        {
            _context = context;
            _securityUtility = securityUtility;
            _indexedEntitysRepository = indexedEntitysRepository;
            _leaguesRepository = new LeaguesRepository(context);
        }

        public async Task<LeagueModel> AddLeagueAsync(LeagueModel league)
        {
            league.AddedById = _securityUtility.GetUserId().Value;
            league.EntityStatus = Enums.EntityStatusEnum.Active;
            _leaguesRepository.Add(league);
            await _context.SaveChangesAsync();
            await _indexedEntitysRepository.Add(_context, league);
            // TODO: log activity
            return league;
        }
    }

    public interface ILeaguesService
    {
        Task<LeagueModel> AddLeagueAsync(LeagueModel league);
    }
}