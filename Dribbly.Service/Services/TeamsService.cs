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
        Task UpdateTeamAsync(TeamModel team);
        Task<TeamViewerDataModel> GetTeamViewerDataAsync(long teamId);
        Task JoinTeamAsync(JoinTeamRequestModel request);
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

        public async Task<UserTeamRelationModel> GetUserTeamRelationAsync(long teamId)
        {
            var currentUserId = _securityUtility.GetUserId();
            long? accountId = await _accountRepo.GetIdentityUserAccountId(currentUserId.Value);
            UserTeamRelationModel relation = new UserTeamRelationModel();
            relation.HasPendingJoinRequest = accountId.HasValue ?
                await GetHasPendingJoinRequestAsync(teamId, accountId.Value) : false;
            return relation;
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
                    throw new DribblyObjectNotFoundException($"Unable to find team with ID {team.Id}.");
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

        public async Task JoinTeamAsync(JoinTeamRequestModel request)
        {
            var currentUserId = _securityUtility.GetUserId();
            long accountId = await _accountRepo.GetIdentityUserAccountIdNotNullAsync(currentUserId.Value);

            if (await GetHasPendingJoinRequestAsync(request.TeamId, accountId))
            {
                throw new DribblyForbiddenException($"Attempted to make multiple reqeusts to join team. Team ID: {request.TeamId}, User ID: {currentUserId}",
                    friendlyMessageKey: "app.Error_JoinTeamDuplicate");
            }

            var team = await GetTeamNotNullAsync(request.TeamId);
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
            _context.JoinTeamRequests.Add(request);
            await _context.SaveChangesAsync();
            await _commonService.AddUserTeamActivity(UserActivityTypeEnum.JoinTeam, team.Id);
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