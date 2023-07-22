using Dribbly.Core.Exceptions;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Courts;
using Dribbly.Model.Entities;
using Dribbly.Model.Games;
using Dribbly.Model.Notifications;
using Dribbly.Model.Shared;
using Dribbly.Model.Teams;
using Dribbly.Model.Tournaments;
using Dribbly.Service.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private readonly ITeamsRepository _teamsRepository;
        private readonly INotificationsRepository _notificationsRepo;

        public TournamentsService(IAuthContext context,
            ISecurityUtility securityUtility) : base(context.Tournaments, context)
        {
            _context = context;
            _securityUtility = securityUtility;
            _tournamentsRepository = new TournamentsRepository(context);
            _teamsRepository = new TeamsRepository(context);
            _notificationsRepo = new NotificationsRepository(context);
        }

        public async Task<TournamentModel> AddTournamentAsync(TournamentModel tournament)
        {
            using (var tx = _context.Database.BeginTransaction())
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

        public async Task<IEnumerable<TournamentModel>> GetNewAsync(GetTournamentsInputModel input)
        {
            return await _context.Tournaments.Include(t => t.Logo)
                .OrderByDescending(t => t.DateAdded)
                .Skip(input.PageSize * (input.Page - 1))
                .Take(input.PageSize)
                .ToListAsync();
        }

        public async Task JoinTournamentAsync(long tournamentId, long teamId)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                try
                {
                    var accountId = _securityUtility.GetAccountId().Value;

                    if (!(await _teamsRepository.IsTeamManagerAsync(teamId, accountId)))
                    {
                        throw new DribblyForbiddenException($"Non-manager tried to sign team up. Team ID: {teamId}, Account ID: {accountId}",
                            friendlyMessage: "Not allowed. Only a manager of the team is allowed to sign the team up.");
                    }
                    else if (await _context.JoinTournamentRequests.AnyAsync(r => r.TournamentId == tournamentId && r.TeamId == teamId))
                    {
                        throw new DribblyInvalidOperationException("A request already exists", friendlyMessage: "A request already exists");
                    }

                    var request = new JoinTournamentRequestModel
                    {
                        TeamId = teamId,
                        TournamentId = tournamentId,
                        AddedByID = accountId,
                        DateAdded = DateTime.UtcNow
                    };

                    _context.JoinTournamentRequests.Add(request);
                    await _context.SaveChangesAsync();

                    request = await _context.JoinTournamentRequests.Include(r => r.Team)
                        .Include(r => r.Tournament).Include(r => r.AddedBy.User)
                        .SingleAsync(r => r.Id == request.Id);

                    _ = AddJoinTournamentNotification(request, NotificationTypeEnum.NewJoinTournamentRequest);

                    await _context.SaveChangesAsync();
                    tx.Commit();
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
            var entity = await _context.Tournaments
                .Include(t => t.Games.Select(g => g.Team1.Team.Logo))
                .Include(t => t.Games.Select(g => g.Team2.Team.Logo))
                .Include(t => t.DefaultCourt.PrimaryPhoto)
                .Include(t => t.Teams.Select(tm => tm.Team.Logo))
                .Include(t => t.JoinRequests)
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            if (entity != null)
            {
                entity.Games = entity.Games.Where(g => g.EntityStatus != Enums.EntityStatusEnum.Deleted).ToList();
                return new TournamentViewerModel(entity);
            }

            return null;
        }

        public NotificationModel AddJoinTournamentNotification(JoinTournamentRequestModel request, NotificationTypeEnum type)
        {
            var notif = new NotificationModel(NotificationTypeEnum.NewJoinTournamentRequest);
            if (type == NotificationTypeEnum.NewJoinTournamentRequest)
            {
                notif.ForUserId = request.Tournament.AddedById;
                notif.AdditionalInfo = JsonConvert.SerializeObject(new
                {
                    requestId = request.Id,
                    teamName = request.Team.Name,
                    teamId = request.TeamId,
                    tournamentName = request.Tournament.Name,
                    tournamentId = request.TournamentId
                });
            }

            _notificationsRepo.TryAddAsync(notif);
            return notif;
        }
    }

    public interface ITournamentsService
    {
        Task<TournamentModel> AddTournamentAsync(TournamentModel season);
        Task<TournamentViewerModel> GetTournamentViewerAsync(long tournamentId);
        Task<IEnumerable<TournamentModel>> GetNewAsync(GetTournamentsInputModel input);
        Task JoinTournamentAsync(long tournamentId, long teamId);
    }
}