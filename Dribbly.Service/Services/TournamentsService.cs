using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Courts;
using Dribbly.Model.Entities;
using Dribbly.Model.Games;
using Dribbly.Model.Shared;
using Dribbly.Model.Teams;
using Dribbly.Model.Tournaments;
using Dribbly.Service.Repositories;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public class TournamentsService : BaseEntityService<TournamentModel>, ITournamentsService
    {
        IAuthContext _context;
        ISecurityUtility _securityUtility;
        private readonly ITournamentsRepository _tournamentsRepository;

        public TournamentsService(IAuthContext context,
            ISecurityUtility securityUtility) : base(context.Tournaments, context)
        {
            _context = context;
            _securityUtility = securityUtility;
            _tournamentsRepository = new TournamentsRepository(context);
        }

        public async Task<TournamentModel> AddTournamentAsync(TournamentModel tournament)
        {
            using(var tx = _context.Database.BeginTransaction())
            {
                try
                {
                    tournament.EntityStatus = Enums.EntityStatusEnum.Active;
                    tournament.AddedById = _securityUtility.GetAccountId().Value;
                    _tournamentsRepository.Add(tournament);
                    await _context.SaveChangesAsync();

                    _context.IndexedEntities.Add(new IndexedEntityModel(tournament));
                    _context.SaveChanges();
                    // TODO: log activity

                    tx.Commit();
                    return tournament;
                }
                catch (System.Exception)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public async Task<TournamentViewerModel> GetTournamentViewerAsync(long tournamentId)
        {
            var entity = await _tournamentsRepository.Get(t => t.Id == tournamentId,
                $"{nameof(TournamentModel.Games)}.{nameof(GameModel.Team1)}.{nameof(GameTeamModel.Team)}.{nameof(TeamModel.Logo)}," +
                $"{nameof(TournamentModel.Games)}.{nameof(GameModel.Team2)}.{nameof(GameTeamModel.Team)}.{nameof(TeamModel.Logo)}," +
                $"{nameof(TournamentModel.DefaultCourt)}.{nameof(CourtModel.PrimaryPhoto)}")
                .FirstOrDefaultAsync();
            if (entity != null)
            {
                entity.Games = entity.Games.Where(g => g.EntityStatus != Enums.EntityStatusEnum.Deleted).ToList();
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