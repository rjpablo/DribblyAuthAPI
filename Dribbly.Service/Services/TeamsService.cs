using Dribbly.Core.Exceptions;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Notifications;
using Dribbly.Model.Teams;
using Dribbly.Service.Enums;
using Dribbly.Service.Repositories;
using Dribbly.Service.Services.Shared;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public interface ITeamsService
    {
        Task<TeamModel> AddTeamAsync(TeamModel team);
        Task CancelJoinRequestAsync(long teamId);
        IEnumerable<TeamModel> GetAll();
        Task<TeamModel> GetTeamAsync(long id);
        Task<UserTeamRelationModel> GetUserTeamRelationAsync(long teamId);
        Task<TeamViewerDataModel> GetTeamViewerDataAsync(long teamId);
        Task<UserTeamRelationModel> JoinTeamAsync(JoinTeamRequestModel request);
        Task<UserTeamRelationModel> LeaveTeamAsync(long teamId);
        Task UpdateTeamAsync(TeamModel team);
    }
    public class TeamsService : BaseEntityService<TeamModel>, ITeamsService
    {
        private readonly IAuthContext _context;
        private readonly ISecurityUtility _securityUtility;
        private readonly IFileService _fileService;
        private readonly IAccountRepository _accountRepo;
        private readonly INotificationsRepository _notificationsRepo;
        private readonly ICourtsRepository _courtsRepo;
        private readonly ICommonService _commonService;
        private readonly IIndexedEntitysRepository _indexedEntitysRepository;

        public TeamsService(IAuthContext context,
            ISecurityUtility securityUtility,
            IAccountRepository accountRepo,
            IFileService fileService,
            INotificationsRepository notificationsRepo,
            ICourtsRepository courtsRepo,
            ICommonService commonService,
            IIndexedEntitysRepository indexedEntitysRepository) : base(context.Teams)
        {
            _context = context;
            _securityUtility = securityUtility;
            _accountRepo = accountRepo;
            _fileService = fileService;
            _notificationsRepo = notificationsRepo;
            _courtsRepo = courtsRepo;
            _commonService = commonService;
            _indexedEntitysRepository = indexedEntitysRepository;
        }

        public IEnumerable<TeamModel> GetAll()
        {
            return All();
        }

        public async Task<TeamViewerDataModel> GetTeamViewerDataAsync(long teamId)
        {
            return new TeamViewerDataModel
            {
                Team = await GetTeamAsync(teamId)
            };
        }

        public async Task<IEnumerable<TeamMembershipViewModel>> GetMembershipsAsync(IAuthContext context, long teamId, params long[] accountIds)
        {
            int IdsCount = accountIds.Length;
            return await context.TeamMembers
                .Select(m => new TeamMembershipViewModel
                {
                    TeamId = m.TeamId,
                    MemberAccountId = m.MemberAccountId,
                    IsCurrentMember = m.DateLeft == null,
                    IsFormerMember = m.DateLeft != null,
                    HasPendingJoinRequest = false,
                    Position = m.Position
                })
                .Union(context.JoinTeamRequests
                .Select(r => new TeamMembershipViewModel
                {
                    TeamId = r.TeamId,
                    MemberAccountId = r.MemberAccountId,
                    IsCurrentMember = false,
                    IsFormerMember = false,
                    HasPendingJoinRequest = r.Status == Model.Enums.JoinTeamRequestStatus.Pending,
                    Position = r.Position
                }))
                .Where(m => m.TeamId == teamId && (IdsCount == 0 || accountIds.Contains(m.MemberAccountId)))
                .ToListAsync();
        }

        public async Task<UserTeamRelationModel> GetUserTeamRelationAsync(long teamId)
        {
            var currentUserId = _securityUtility.GetUserId();
            long? accountId = currentUserId.HasValue ? await _accountRepo.GetIdentityUserAccountId(currentUserId.Value) : null;
            return await GetUserTeamRelationAsync(teamId, accountId.Value);
        }

        public async Task<UserTeamRelationModel> GetUserTeamRelationAsync(long teamId, long accountId)
        {
            UserTeamRelationModel relation = new UserTeamRelationModel();
            IEnumerable<TeamMembershipViewModel> memberships = await GetMembershipsAsync(_context, teamId, accountId);
            relation.IsCurrentMember = memberships.Any(m => m.MemberAccountId == accountId && m.IsCurrentMember);
            relation.IsFormerMember = memberships.Any(m => m.MemberAccountId == accountId && m.IsFormerMember);
            relation.HasPendingJoinRequest = memberships.Any(m => m.MemberAccountId == accountId && m.HasPendingJoinRequest);
            relation.IsCurrentCoach = memberships.Any(m => m.MemberAccountId == accountId && m.IsCurrentMember && m.Position == PlayerPositionEnum.Coach);
            return relation;
        }

        public async Task<UserTeamRelationModel> LeaveTeamAsync(long teamId)
        {
            var currentUserId = _securityUtility.GetUserId();
            if (!currentUserId.HasValue)
            {
                throw new UnauthorizedAccessException("Unauthenticated user attempted to leave a team.");
            }
            long accountId = await _accountRepo.GetIdentityUserAccountIdNotNullAsync(currentUserId.Value);
            var activeMembership = _context.TeamMembers.SingleOrDefault(m => m.TeamId == teamId && m.MemberAccountId == accountId &&
            m.DateLeft == null);
            if(activeMembership == null)
            {
                throw new DribblyForbiddenException("Attempted to leave team but not currently a member.",
                    friendlyMessageKey: "app.LeaveTeamNotCurrentMember");
            }

            activeMembership.DateLeft = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return await GetUserTeamRelationAsync(teamId, accountId);
        }

        public async Task<TeamModel> GetTeamAsync(long id)
        {
            TeamModel team = await _dbSet.Where(g => g.Id == id).Include(g => g.HomeCourt).Include(t => t.Logo)
                .Include(g => g.HomeCourt.PrimaryPhoto).SingleOrDefaultAsync();
            if (team != null)
            {
                team.AddedBy = await _accountRepo.GetAccountBasicInfo(team.AddedById);
                if (team.AddedBy == null)
                {
                    throw new DribblyObjectNotFoundException($"Unable to find user with ID {team.AddedById}.");
                }
                team.ManagedBy = await _accountRepo.GetAccountBasicInfo(team.ManagedById);
                if (team.ManagedBy == null)
                {
                    throw new DribblyObjectNotFoundException($"Unable to find user with ID {team.ManagedById}.");
                }
            }
            return team;
        }

        public async Task<JoinTeamRequestModel> GetPendingJoinRequestAsync(long teamId, long accountId)
        {
            return await _context.JoinTeamRequests.FirstOrDefaultAsync(r => r.TeamId == teamId && r.MemberAccountId == accountId &&
            r.Status == Model.Enums.JoinTeamRequestStatus.Pending);
        }

        public async Task CancelJoinRequestAsync(long teamId)
        {
            var currentUserId = _securityUtility.GetUserId();
            long accountId = await _accountRepo.GetIdentityUserAccountIdNotNullAsync(currentUserId.Value);
            JoinTeamRequestModel request = await GetPendingJoinRequestAsync(teamId, accountId);

            if (request == null)
            {
                throw new DribblyForbiddenException($"Attempted to cancel request to join team but no pending request was found. Team ID: {teamId}, User ID: {currentUserId}",
                    friendlyMessageKey: "app.Error_JoinTeamCancelNoPending");
            }

            request.Status = Model.Enums.JoinTeamRequestStatus.Cancelled;
            _context.JoinTeamRequests.AddOrUpdate(request);
            await _context.SaveChangesAsync();
        }

        public async Task<TeamModel> GetTeamNotNullAsync(long id)
        {
            var team = await GetTeamAsync(id);
            if (team == null)
            {
                throw new DribblyObjectNotFoundException($"Unable to find team with ID {id}.");
            }
            return team;
        }

        public async Task<TeamModel> AddTeamAsync(TeamModel team)
        {
            var currentUserId = _securityUtility.GetUserId();
            team.AddedById = currentUserId.Value;
            team.ManagedById = currentUserId.Value;
            team.EntityStatus = EntityStatusEnum.Active;
            Add(team);
            _context.SaveChanges();
            await _indexedEntitysRepository.Add(_context, team);
            await _commonService.AddUserTeamActivity(UserActivityTypeEnum.AddTeam, team.Id);

            return team;
        }

        public async Task<bool> GetHasPendingJoinRequestAsync(long teamId, long accountId)
        {
            return (await GetPendingJoinRequestAsync(teamId, accountId)) != null;
        }

        public async Task<TeamMembershipModel> AddMemberAsync(JoinTeamRequestModel request)
        {
            var membership = new TeamMembershipModel
            {
                MemberAccountId = request.MemberAccountId,
                Position = request.Position,
                TeamId = request.TeamId,
                DateAdded = DateTime.UtcNow
            };

            _context.TeamMembers.Add(membership);
            await _context.SaveChangesAsync();
            return membership;
        }

        public async Task<UserTeamRelationModel> JoinTeamAsync(JoinTeamRequestModel request)
        {
            var currentUserId = _securityUtility.GetUserId();
            long accountId = await _accountRepo.GetIdentityUserAccountIdNotNullAsync(currentUserId.Value);

            if (await GetHasPendingJoinRequestAsync(request.TeamId, accountId))
            {
                throw new DribblyForbiddenException($"Attempted to make multiple reqeusts to join team. Team ID: {request.TeamId}, User ID: {currentUserId}",
                    friendlyMessageKey: "app.Error_JoinTeamDuplicate");
            }

            var team = await GetTeamNotNullAsync(request.TeamId);
            var isManager = team.ManagedById == currentUserId;
            if (team.EntityStatus == EntityStatusEnum.Inactive)
            {
                throw new DribblyForbiddenException($"Attempted to join an inactive team. Team ID: {request.TeamId}, User ID: {currentUserId}",
                    friendlyMessageKey: "app.Error_JoinTeamInactive");
            }
            if (team.EntityStatus == EntityStatusEnum.Deleted)
            {
                throw new DribblyForbiddenException($"Attempted to join an already deleted team. Team ID: {request.TeamId}, User ID: {currentUserId}",
                    friendlyMessageKey: "app.Error_JoinTeamDeleted");
            }
            if (GetCurrentTeamMemberships(request.TeamId, accountId).Count() > 0)
            {
                throw new DribblyForbiddenException($"Current member of a team attempted to join the same team. Team ID: {request.TeamId}, Account ID: {accountId}",
                    friendlyMessageKey: "app.Error_JoinTeamAlreadyAMember");
            }

            request.MemberAccountId = accountId;
            request.DateAdded = DateTime.UtcNow;

            if (isManager) // immediately add as member if manager
            {
                await AddMemberAsync(request);
                await _commonService.AddUserTeamActivity(UserActivityTypeEnum.JoinTeam, team.Id);
            }
            else
            {
                _context.JoinTeamRequests.Add(request);
                await _context.SaveChangesAsync();
                await _commonService.AddUserTeamActivity(UserActivityTypeEnum.RequestToJoinTeam, team.Id);
                await _notificationsRepo.TryAddAsync(new JoinTeamRequestNotificationModel
                {
                    RequestId = request.Id,
                    ForUserId = team.ManagedById,
                    DateAdded = DateTime.UtcNow,
                    Type = NotificationTypeEnum.JoinTeamRequest
                });
            }
            return await GetUserTeamRelationAsync(team.Id);
        }

        public IQueryable<TeamMembershipModel> GetCurrentTeamMemberships(long teamId, long memberAccountId)
        {
            return _context.TeamMembers.Where(m => m.DateLeft == null && m.TeamId == teamId && m.MemberAccountId == memberAccountId);
        }

        public async Task UpdateTeamAsync(TeamModel team)
        {
            Update(team);
            var currentUserId = _securityUtility.GetUserId();
            NotificationTypeEnum Type = team.AddedById == currentUserId ?
                NotificationTypeEnum.NewBookingForOwner :
                NotificationTypeEnum.NewBookingForBooker;
            _context.SaveChanges();
            await _commonService.AddUserTeamActivity(UserActivityTypeEnum.UpdateTeam, team.Id);
        }
    }
}