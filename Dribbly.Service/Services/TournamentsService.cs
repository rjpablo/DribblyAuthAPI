using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Leagues;
using Dribbly.Model.Tournaments;
using Dribbly.Service.Repositories;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public class TournamentsService : BaseEntityService<TournamentModel>, ITournamentsService
    {
        IAuthContext _context;
        ISecurityUtility _securityUtility;
        private readonly IIndexedEntitysRepository _indexedEntitysRepository;
        private readonly ITournamentsRepository _tournamentsRepository;

        public TournamentsService(IAuthContext context,
            ISecurityUtility securityUtility,
            IIndexedEntitysRepository indexedEntitysRepository) : base(context.Tournaments)
        {
            _context = context;
            _securityUtility = securityUtility;
            _indexedEntitysRepository = indexedEntitysRepository;
            _tournamentsRepository = new TournamentsRepository(context);
        }

        public async Task<TournamentModel> AddTournamentAsync(TournamentModel season)
        {
            season.AddedById = _securityUtility.GetUserId().Value;
            _tournamentsRepository.Add(season);
            await _context.SaveChangesAsync();
            // TODO: log activity
            return season;
        }

        public async Task<TournamentViewerModel> GetTournamentViewerAsync(long tournamentId)
        {
            var entity = await _tournamentsRepository.Get(t => t.Id == tournamentId).FirstOrDefaultAsync();
            if (entity != null)
            {
                return new TournamentViewerModel(entity);
            }

            return null;
        }
    }

    public interface ITournamentsService
    {
        Task<TournamentModel> AddTournamentAsync(TournamentModel season);
        Task<TournamentViewerModel> GetTournamentViewerAsync(long tournamentId);
    }
}