using Dribbly.Core.Exceptions;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Courts;
using Dribbly.Model.DTO;
using Dribbly.Model.Entities;
using Dribbly.Model.Enums;
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

        #region Stages and Brackets

        public async Task AddTournamentStageAsync(AddTournamentStageInputModel input)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                try
                {
                    var accountId = _securityUtility.GetAccountId().Value;
                    var stage = new TournamentStageModel
                    {
                        Name = input.StageName,
                        TournamentId = input.TournamentId,
                        AddedById = accountId,
                        DateAdded = DateTime.UtcNow,
                        Status = StageStatusEnum.NotStarted
                    };

                    _context.TournamentStages.Add(stage);
                    await _context.SaveChangesAsync();

                    for (int i = 0; i < input.BracketsCount; i++)
                    {
                        _context.StageBrackets.Add(new StageBracketModel
                        {
                            StageId = stage.Id,
                            AddedById = accountId,
                            DateAdded = DateTime.UtcNow,
                            Name = "Bracket " + ((char)(65 + i)).ToString()
                        });
                    }

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

        public async Task<TournamentStageModel> SetStageTeamsAsync(SetStageTeamsInputModel input)
        {
            var stage = await _context.TournamentStages.Include(s => s.Teams.Select(t => t.Team))
                .SingleOrDefaultAsync(s => s.Id == input.StageId);

            // remove teams that are not selected
            var toRemove = stage.Teams.Where(t => !input.TeamIds.Contains(t.TeamId)).ToList();
            foreach (var team in toRemove)
            {
                _context.StageTeams.Remove(team);
            }

            // add newly added teams
            foreach (long id in input.TeamIds)
            {
                if (!stage.Teams.Any(t => t.TeamId == id))
                {
                    stage.Teams.Add(new StageTeamModel()
                    {
                        TeamId = id,
                        StageId = input.StageId,
                        DateAdded = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            return await _context.TournamentStages.Include(s => s.Teams.Select(t => t.Team.Logo))
                .SingleOrDefaultAsync(s => s.Id == input.StageId);
        }

        public async Task<IEnumerable<TournamentStageModel>> GetTournamentStagesAsync(long tournamentId)
        {
            return await _context.TournamentStages.Include(s => s.Brackets).Include(s => s.Teams.Select(t=>t.Team.Logo))
                .Where(s => s.TournamentId == tournamentId).ToListAsync();
        }

        public async Task SetTeamBracket(long teamId, long stageId, long? bracketId)
        {
            var team = await _context.StageTeams.SingleOrDefaultAsync(t => t.TeamId == teamId && t.StageId == stageId);
            team.BracketId = bracketId;
            await _context.SaveChangesAsync();
        }

        #endregion

        public async Task<IEnumerable<TournamentModel>> GetNewAsync(GetTournamentsInputModel input)
        {
            return await _context.Tournaments.Include(t => t.Logo)
                .OrderByDescending(t => t.DateAdded)
                .Skip(input.PageSize * (input.Page - 1))
                .Take(input.PageSize)
                .ToListAsync();
        }

        #region Join Requests
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
                    else if (await _context.TournamentTeams.AnyAsync(r => r.TournamentId == tournamentId && r.TeamId == teamId))
                    {
                        throw new DribblyInvalidOperationException($"Team is already approved to join tournament. Team ID: {teamId}, Tournament ID: {tournamentId}", friendlyMessage: "This team has already been to approved to join.");
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

        public async Task<TeamStatsViewModel> ProcessJoinRequestAsync(long requestId, bool shouldApprove)
        {
            var accountId = _securityUtility.GetAccountId().Value;
            var request = await _context.JoinTournamentRequests.Include(r => r.Team)
                        .Include(r => r.Tournament).Include(r => r.AddedBy.User)
                        .SingleAsync(r => r.Id == requestId);

            if (request == null)
            {
                throw new DribblyInvalidOperationException("Join tournament request not found.",
                    friendlyMessage: "The request no longer exists. I may have been cancelled or already processed.");
            }
            else if (request.Tournament.AddedById != accountId)
            {
                throw new DribblyForbiddenException($"Non-tournament manager tried to process join request. Request ID: {requestId}, Account ID: {accountId}",
                    friendlyMessage: "Forbidden! You do not have sufficient access to perform this action.");
            }

            TeamStatsViewModel result = null;
            if (shouldApprove)
            {
                var tournamentTeam = new TournamentTeamModel
                {
                    TeamId = request.TeamId,
                    TournamentId = request.TournamentId,
                    DateAdded = DateTime.UtcNow
                };
                _context.TournamentTeams.Add(tournamentTeam);
                _ = AddJoinTournamentNotification(request, NotificationTypeEnum.JoinTournamentRequestApproved);
                await _context.SaveChangesAsync();

                tournamentTeam = await _context.TournamentTeams.SingleAsync(t => t.Id == tournamentTeam.Id);
                result = new TeamStatsViewModel(tournamentTeam);

            }
            else
            {
                _ = AddJoinTournamentNotification(request, NotificationTypeEnum.JoinTournamentRequestRejected);
            }

            _context.JoinTournamentRequests.Remove(request);
            await _context.SaveChangesAsync();
            return result;
        }

        public async Task RemoveTournamentTeamAsync(long tournamentId, long teamId)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                try
                {
                    var team = await _context.TournamentTeams
                        .Include(t => t.Tournament).Include(t => t.Team)
                        .SingleOrDefaultAsync(r => r.TournamentId == tournamentId && r.TeamId == teamId);

                    if (team == null)
                    {
                        // TODO: log info "tournament team to remove not found"
                        // no need to throw exception
                        return;
                    }


                    var notif = new NotificationModel(NotificationTypeEnum.TournamentTeamRemoved);
                    notif.ForUserId = team.Team.ManagedById;
                    notif.AdditionalInfo = JsonConvert.SerializeObject(new
                    {
                        teamName = team.Team.Name,
                        teamId = team.TeamId,
                        tournamentName = team.Tournament.Name,
                        tournamentId = team.TournamentId
                    });

                    await _notificationsRepo.TryAddAsync(notif);

                    _context.TournamentTeams.Remove(team);
                    await _context.SaveChangesAsync();
                    tx.Commit();
                }
                catch (Exception)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public NotificationModel AddJoinTournamentNotification(JoinTournamentRequestModel request, NotificationTypeEnum type)
        {
            var notif = new NotificationModel(type);
            notif.AdditionalInfo = JsonConvert.SerializeObject(new
            {
                requestId = request.Id,
                teamName = request.Team.Name,
                teamId = request.TeamId,
                tournamentName = request.Tournament.Name,
                tournamentId = request.TournamentId
            });

            switch (notif.Type)
            {
                case NotificationTypeEnum.NewJoinTournamentRequest:
                    notif.ForUserId = request.Tournament.AddedById;
                    break;
                case NotificationTypeEnum.JoinTournamentRequestApproved:
                case NotificationTypeEnum.JoinTournamentRequestRejected:
                    notif.ForUserId = request.Team.ManagedById;
                    break;
            }

            _notificationsRepo.TryAddAsync(notif);
            return notif;
        }
        #endregion

        public async Task<TournamentViewerModel> GetTournamentViewerAsync(long tournamentId)
        {
            var entity = await _context.Tournaments
                .Include(t => t.Games.Select(g => g.Team1.Team.Logo))
                .Include(t => t.Games.Select(g => g.Team2.Team.Logo))
                .Include(t => t.DefaultCourt.PrimaryPhoto)
                .Include(t => t.Teams.Select(tm => tm.Team.Logo))
                .Include(t => t.JoinRequests.Select(r => r.Team))
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

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
        Task<IEnumerable<TournamentModel>> GetNewAsync(GetTournamentsInputModel input);
        Task RemoveTournamentTeamAsync(long tournamentId, long teamId);

        #region Stages and Brackets
        Task AddTournamentStageAsync(AddTournamentStageInputModel input);
        Task<IEnumerable<TournamentStageModel>> GetTournamentStagesAsync(long tournamentId);
        Task<TournamentStageModel> SetStageTeamsAsync(SetStageTeamsInputModel input);
        Task SetTeamBracket(long teamId, long stageId, long? bracketId);
        #endregion

        #region Join Requests
        Task JoinTournamentAsync(long tournamentId, long teamId);
        Task<TeamStatsViewModel> ProcessJoinRequestAsync(long requestId, bool shouldApprove);
        #endregion
    }
}