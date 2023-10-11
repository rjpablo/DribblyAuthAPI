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
using System.Data.Entity.Infrastructure;
using Dribbly.Model.Courts;
using Dribbly.Authentication.Services;
using System.Web;
using Dribbly.Core.Enums.Permissions;
using Dribbly.Model.Account;
using Dribbly.Model.DTO;
using Dribbly.Core.Models;
using Dribbly.Model.Shared;
using Dribbly.Model.Entities;
using Dribbly.Model.Enums;
using Dribbly.Core.Enums;
using Dribbly.Chat.Services;
using Dribbly.Chat.Models;
using Dribbly.Chat.Models.ViewModels;
using Newtonsoft.Json;

namespace Dribbly.Service.Services
{
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
        private readonly IDribblyChatService _dribblyChatService;

        public TeamsService(IAuthContext context,
            ISecurityUtility securityUtility,
            IAccountRepository accountRepo,
            IFileService fileService,
            INotificationsRepository notificationsRepo,
            ICourtsRepository courtsRepo,
            IIndexedEntitysRepository indexedEntitysRepository) : base(context.Teams, context)
        {
            _context = context;
            _securityUtility = securityUtility;
            _accountRepo = accountRepo;
            _fileService = fileService;
            _notificationsRepo = notificationsRepo;
            _courtsRepo = courtsRepo;
            _commonService = new CommonService(context, securityUtility);
            _indexedEntitysRepository = indexedEntitysRepository;
            _dribblyChatService = new DribblyChatService(context);
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
                    HasPendingJoinRequest = false
                })
                .Union(context.JoinTeamRequests
                .Select(r => new TeamMembershipViewModel
                {
                    TeamId = r.TeamId,
                    MemberAccountId = r.MemberAccountId,
                    IsCurrentMember = false,
                    IsFormerMember = false,
                    HasPendingJoinRequest = r.Status == Model.Enums.JoinTeamRequestStatus.Pending
                }))
                .Where(m => m.TeamId == teamId && (IdsCount == 0 || accountIds.Contains(m.MemberAccountId)))
                .ToListAsync();
        }

        public async Task<UserTeamRelationModel> GetUserTeamRelationAsync(long teamId)
        {
            long? accountId = _securityUtility.GetAccountId();
            return accountId.HasValue ? await GetUserTeamRelationAsync(teamId, accountId.Value) : new UserTeamRelationModel();
        }

        public async Task<IEnumerable<TeamMembershipModel>> GetCurrentMembersAsync(long teamId)
        {
            return await GetAllMembers(teamId).Where(m => m.DateLeft == null).ToListAsync();
        }

        public async Task<ChatRoomViewModel> GetTeamChatAsync(long teamId)
        {
            var accountId = _securityUtility.GetAccountId().Value;
            string code = "tm" + teamId.ToString();
            var chat = await _dribblyChatService.GetChatByCodeAsync(code);

            if (chat == null)
            {
                var team = await _context.Teams
                    .Include(t => t.Members)
                    .SingleOrDefaultAsync(t => t.Id == teamId);
                if (team == null)
                {
                    throw new DribblyObjectNotFoundException($"Coulnd't find team with ID {teamId}",
                        friendlyMessage: "Unable to find team information.");
                }

                var input = new CreateChatInpuModel
                {
                    Code = code,
                    Title = team.Name,
                    Type = Chat.Enums.ChatTypeEnum.Team,
                    IconId = team.LogoId
                };

                input.ParticipantIds.Add(accountId);
                foreach (var m in team.Members)
                {
                    input.ParticipantIds.Add(m.MemberAccountId);
                }
                input.ParticipantIds = input.ParticipantIds.Distinct().ToList();

                chat = await _dribblyChatService.CreateChatAsync(input, accountId);
            }

            return new ChatRoomViewModel(chat, accountId);
        }

        public async Task RemoveMemberAsync(long teamId, long membershipId)
        {
            //TODO: validate team existence and link to memebrshipId
            using (var tx = _context.Database.BeginTransaction())
            {
                try
                {
                    var member = await _context.TeamMembers.SingleOrDefaultAsync(m => m.Id == membershipId);
                    if (member == null)
                    {
                        throw new DribblyForbiddenException("Membership info not found.", friendlyMessage: "Unable to find membership info.");
                    }
                    member.DateLeft = DateTime.UtcNow;
                    await _dribblyChatService.RemoveChatParticipant("tm" + teamId, member.MemberAccountId);
                    await _context.SaveChangesAsync();
                    tx.Commit();
                }
                catch (Exception e)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public async Task ProcessJoinRequestAsync(ProcessJoinTeamRequestInputModel input)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                try
                {
                    var team = await GetTeamNotNullAsync(input.Request.TeamId);
                    if (_securityUtility.GetAccountId() != team.ManagedById)
                    {
                        throw new DribblyForbiddenException("Non-manager of team attempted to process a join team request.");
                    }

                    var isRequestPending = (await _context.JoinTeamRequests.FindAsync(input.Request.Id))?.Status == Model.Enums.JoinTeamRequestStatus.Pending;
                    if (!isRequestPending)
                    {
                        throw new DribblyForbiddenException("Attempted to process a member request multiple times.",
                            friendlyMessageKey: "app.Eror_ProcessJoinTeamRequestAlreadyProcessed");
                    }

                    if (input.ShouldApprove)
                    {
                        AddMember(input.Request);
                        input.Request.Status = Model.Enums.JoinTeamRequestStatus.Approved;
                        await _commonService.AddUserJoinTeamRequestActivity(UserActivityTypeEnum.ApprovMemberRequest, input.Request.Id);
                        await _notificationsRepo.TryAddAsync(new NotificationModel
                        {
                            ForUserId = input.Request.MemberAccountId,
                            DateAdded = DateTime.UtcNow,
                            Type = NotificationTypeEnum.JoinTeamRequestApproved,
                            AdditionalInfo = JsonConvert.SerializeObject(new
                            {
                                requestId = input.Request.Id,
                                teamName = team.Name,
                                teamId = team.Id
                            })
                        });
                        await _dribblyChatService.AddChatParticipant("tm" + input.Request.TeamId, input.Request.MemberAccountId);
                    }
                    else
                    {
                        input.Request.Status = Model.Enums.JoinTeamRequestStatus.Denied;
                        await _commonService.AddUserJoinTeamRequestActivity(UserActivityTypeEnum.RejectMemberRequest, input.Request.Id);
                    }

                    _context.JoinTeamRequests.AddOrUpdate(input.Request);
                    await _context.SaveChangesAsync();
                    tx.Commit();
                }
                catch (Exception e)
                {
                    // TODO: log error
                    tx.Rollback();
                    throw;
                }
            }
        }

        public async Task<IEnumerable<JoinTeamRequestModel>> GetJoinRequestsAsync(long teamId)
        {
            var team = _context.Teams.Find(teamId);
            if (team == null)
            {
                throw new DribblyObjectNotFoundException($"Unable to find team with ID {teamId}.");
            }

            if (team.ManagedById != _securityUtility.GetAccountId())
            {
                throw new DribblyForbiddenException("Non-manager of team attempted to access team's join requests.");
            }

            return await _context.JoinTeamRequests.Include(m => m.Member).Include(m => m.Member.User)
               .Include(m => m.Member.ProfilePhoto).Where(m => m.TeamId == teamId && m.Status == Model.Enums.JoinTeamRequestStatus.Pending)
               .ToListAsync();
        }

        public async Task<IEnumerable<TeamStatsViewModel>> GetTopTeamsAsync(PagedGetInputModel input)
        {
            return (await _context.TeamStats.Include(s => s.Team.Logo)
                .OrderByDescending(s => s.OverallScore)
                .ThenBy(s => s.Team.Name)
                .Skip(input.PageSize * (input.Page - 1))
                .ToListAsync()).Select(s => new TeamStatsViewModel(s));
        }

        #region GetTeams
        public async Task<IEnumerable<TeamStatsViewModel>> GetTeamsAsync(GetTeamsFilterModel filter)
        {
            var query = _context.TeamStats
                .Include(s => s.Team.Logo)
                .Where(s => !filter.CourtIds.Any() || (s.Team.HomeCourtId.HasValue && filter.CourtIds.Contains(s.Team.HomeCourtId.Value)));
            query = ApplySortingAndPaging(query, filter);
            var players = await query.ToListAsync();
            return players.Select(s => new TeamStatsViewModel(s));
        }

        private IQueryable<TeamStatsModel> ApplySortingAndPaging(IQueryable<TeamStatsModel> query, GetTeamsFilterModel filter)
        {
            IOrderedQueryable<TeamStatsModel> ordered = null;
            bool isAscending = filter.SortDirection == SortDirectionEnum.Ascending;
            switch (filter.SortBy)
            {
                case StatEnum.PPG:
                    ordered = isAscending ? query.OrderBy(s => s.PPG) : query.OrderByDescending(s => s.PPG);
                    break;
                case StatEnum.RPG:
                    ordered = isAscending ? query.OrderBy(s => s.RPG) : query.OrderByDescending(s => s.RPG);
                    break;
                case StatEnum.APG:
                    ordered = isAscending ? query.OrderBy(s => s.APG) : query.OrderByDescending(s => s.APG);
                    break;
                case StatEnum.FGP:
                    ordered = isAscending ? query.OrderBy(s => s.FGP) : query.OrderByDescending(s => s.FGP);
                    break;
                case StatEnum.BPG:
                    ordered = isAscending ? query.OrderBy(s => s.BPG) : query.OrderByDescending(s => s.BPG);
                    break;
                case StatEnum.TPG:
                    ordered = isAscending ? query.OrderBy(s => s.TPG) : query.OrderByDescending(s => s.TPG);
                    break;
                case StatEnum.SPG:
                    ordered = isAscending ? query.OrderBy(s => s.SPG) : query.OrderByDescending(s => s.SPG);
                    break;
                case StatEnum.ThreePP:
                    ordered = isAscending ? query.OrderBy(s => s.ThreePP) : query.OrderByDescending(s => s.ThreePP);
                    break;
                default:
                    ordered = isAscending ? query.OrderBy(s => s.OverallScore) : query.OrderByDescending(s => s.OverallScore);
                    break;
            }
            return ordered.Skip(filter.PageSize * (filter.Page - 1))
                .Take(filter.PageSize);
        }
        #endregion

        public IQueryable<TeamMembershipModel> GetAllMembers(long teamId)
        {
            return _context.TeamMembers.Include(m => m.Account).Include(m => m.Account.User)
                .Include(m => m.Account.ProfilePhoto).Where(m => m.TeamId == teamId);
        }

        public async Task<IEnumerable<ChoiceItemModel<long>>> GetManagedTeamsAsChoicesAsync()
        {
            var currentAccountId = _securityUtility.GetAccountId();
            return (await _context.Teams.Where(t => t.ManagedById == currentAccountId && t.EntityStatus == EntityStatusEnum.Active)
                .ToListAsync())
                .Select(t => new ChoiceItemModel<long>(t.Name, t.Id, t.Logo?.Url, EntityTypeEnum.Team));
        }

        public async Task<UserTeamRelationModel> GetUserTeamRelationAsync(long teamId, long accountId)
        {
            UserTeamRelationModel relation = new UserTeamRelationModel();
            IEnumerable<TeamMembershipViewModel> memberships = await GetMembershipsAsync(_context, teamId, accountId);
            relation.IsCurrentMember = memberships.Any(m => m.MemberAccountId == accountId && m.IsCurrentMember);
            relation.IsFormerMember = memberships.Any(m => m.MemberAccountId == accountId && m.IsFormerMember);
            relation.HasPendingJoinRequest = memberships.Any(m => m.MemberAccountId == accountId && m.HasPendingJoinRequest);
            relation.IsCurrentCoach = memberships.Any(m => m.MemberAccountId == accountId && m.IsCurrentMember);
            return relation;
        }

        public async Task<UserTeamRelationModel> LeaveTeamAsync(long teamId)
        {
            long accountId = _securityUtility.GetAccountId().Value;
            var activeMembership = _context.TeamMembers.SingleOrDefault(m => m.TeamId == teamId && m.MemberAccountId == accountId &&
            m.DateLeft == null);
            if (activeMembership == null)
            {
                throw new DribblyForbiddenException("Attempted to leave team but not currently a member.",
                    friendlyMessageKey: "app.LeaveTeamNotCurrentMember");
            }

            activeMembership.DateLeft = DateTime.UtcNow;
            using (var tx = _context.Database.BeginTransaction())
            {
                try
                {
                    await _dribblyChatService.RemoveChatParticipant("tm" + teamId, accountId);
                    await _context.SaveChangesAsync();
                    tx.Commit();
                    return await GetUserTeamRelationAsync(teamId, accountId);
                }
                catch (Exception e)
                {
                    //TODO: log error
                    tx.Rollback();
                    throw;
                }
            }
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

        public async Task<MultimediaModel> UploadLogoAsync(long teamId)
        {
            TeamModel team = GetById(teamId);

            if ((_securityUtility.GetAccountId() != team.ManagedById) && !AuthenticationService.HasPermission(AccountPermission.UpdatePhotoNotOwned))
            {
                throw new UnauthorizedAccessException("Authorization failed when attempting to update account primary photo.");
            }

            HttpFileCollection files = HttpContext.Current.Request.Files;
            string uploadPath = _fileService.Upload(files[0], "accountPhotos/");

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    MultimediaModel photo = await AddPhoto(team, files[0]);
                    team.LogoId = photo.Id;
                    Update(team);
                    await _context.SaveChangesAsync();
                    await _indexedEntitysRepository.SetIconUrl(_context, team, photo.Url);
                    await _commonService.AddTeamPhotoActivitiesAsync(UserActivityTypeEnum.AddTeamPhoto, team.Id, photo);
                    await _commonService.AddTeamPhotoActivitiesAsync(UserActivityTypeEnum.SetTeamLogo, team.Id, photo);
                    transaction.Commit();
                    return photo;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        private async Task<MultimediaModel> AddPhoto(TeamModel team, HttpPostedFile file)
        {
            string uploadPath = _fileService.Upload(file, "teamLogos/");

            MultimediaModel photo = new MultimediaModel
            {
                Url = uploadPath,
                UploadedById = _securityUtility.GetAccountId().Value,
                DateAdded = DateTime.UtcNow
            };
            _context.Multimedia.Add(photo);
            await _context.SaveChangesAsync();

            _context.TeamPhotos.Add(new TeamPhotoModel
            {
                PhotoId = photo.Id,
                TeamId = team.Id
            });
            await _context.SaveChangesAsync();

            return photo;
        }

        public async Task<JoinTeamRequestModel> GetPendingJoinRequestAsync(long teamId, long accountId)
        {
            return await _context.JoinTeamRequests.FirstOrDefaultAsync(r => r.TeamId == teamId && r.MemberAccountId == accountId &&
            r.Status == Model.Enums.JoinTeamRequestStatus.Pending);
        }

        public async Task CancelJoinRequestAsync(long teamId)
        {
            long? accountId = _securityUtility.GetAccountId();
            JoinTeamRequestModel request = await GetPendingJoinRequestAsync(teamId, accountId.Value);

            if (request == null)
            {
                throw new DribblyForbiddenException($"Attempted to cancel request to join team but no pending request was found. Team ID: {teamId}, Account ID: {accountId}",
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
            var accountId = _securityUtility.GetAccountId().Value;
            team.AddedById = accountId;
            team.ManagedById = accountId;
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

        public TeamMembershipModel AddMember(JoinTeamRequestModel request)
        {
            var membership = new TeamMembershipModel
            {
                MemberAccountId = request.MemberAccountId,
                TeamId = request.TeamId,
                DateAdded = DateTime.UtcNow,
                JerseyNo = request.JerseyNo
            };

            _context.TeamMembers.Add(membership);
            return membership;
        }

        public async Task<UserTeamRelationModel> JoinTeamAsync(JoinTeamRequestInputModel input)
        {
            var currentUserId = _securityUtility.GetUserId();
            var accountId = _securityUtility.GetAccountId().Value;
            var account = _context.Accounts
                .Single(a => a.Id == accountId);
            JoinTeamRequestModel request = new JoinTeamRequestModel(input.TeamId, accountId, input.JerseyNo);

            if (await GetHasPendingJoinRequestAsync(request.TeamId, accountId))
            {
                throw new DribblyForbiddenException($"Attempted to make multiple reqeusts to join team. Team ID: {request.TeamId}, User ID: {currentUserId}",
                    friendlyMessageKey: "app.Error_JoinTeamDuplicate");
            }

            var team = await GetTeamNotNullAsync(request.TeamId);
            var isManager = team.ManagedById == _securityUtility.GetAccountId();
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
                AddMember(request);
                await _context.SaveChangesAsync();
                await _commonService.AddUserTeamActivity(UserActivityTypeEnum.JoinTeam, team.Id);
            }
            else
            {
                _context.JoinTeamRequests.Add(request);
                await _context.SaveChangesAsync();
                await _commonService.AddUserTeamActivity(UserActivityTypeEnum.RequestToJoinTeam, team.Id);
                await _notificationsRepo.TryAddAsync(new NotificationModel
                {
                    ForUserId = team.ManagedById,
                    DateAdded = DateTime.UtcNow,
                    Type = NotificationTypeEnum.JoinTeamRequest,
                    AdditionalInfo = JsonConvert.SerializeObject(new
                    {
                        requestId = request.Id,
                        requestorName = account.Name,
                        teamName = team.Name,
                        teamId = team.Id
                    })
                });
            }
            return await GetUserTeamRelationAsync(team.Id);
        }

        public IQueryable<TeamMembershipModel> GetCurrentTeamMemberships(long teamId, long memberAccountId)
        {
            return _context.TeamMembers.Where(m => m.DateLeft == null && m.TeamId == teamId && m.MemberAccountId == memberAccountId);
        }

        public async Task<IEnumerable<TeamMembershipModel>> GetTopPlayersAsync(long teamId)
        {
            return await _context.TeamMembers
                .Include(s => s.Account.User).Include(s => s.Account.ProfilePhoto)
                .Where(m => m.DateLeft == null && m.TeamId == teamId && m.OverallScore > 0)
                .OrderByDescending(m => m.OverallScore).Take(5).ToListAsync();
        }

        public async Task UpdateTeamAsync(UpdateTeamInputModel input)
        {
            var team = _context.Teams.SingleOrDefault(t => t.Id == input.Id);
            if (team == null)
            {
                throw new DribblyObjectNotFoundException("Team not fould",
                    friendlyMessage: "The team's details could not be found. It may have been deleted from the system");
            }
            team.Name = input.Name;
            team.ShortName = input.ShortName;
            Update(team);
            var currentAccountId = _securityUtility.GetAccountId().Value;
            NotificationTypeEnum Type = team.AddedById == currentAccountId ?
                NotificationTypeEnum.NewGameForOwner :
                NotificationTypeEnum.NewGameForBooker;
            _context.SaveChanges();
            await _commonService.AddUserTeamActivity(UserActivityTypeEnum.UpdateTeam, team.Id);
        }
    }
    public interface ITeamsService
    {
        Task<TeamModel> AddTeamAsync(TeamModel team);
        Task CancelJoinRequestAsync(long teamId);
        IEnumerable<TeamModel> GetAll();
        Task<IEnumerable<TeamMembershipModel>> GetCurrentMembersAsync(long teamId);
        Task<IEnumerable<JoinTeamRequestModel>> GetJoinRequestsAsync(long teamId);
        Task<IEnumerable<ChoiceItemModel<long>>> GetManagedTeamsAsChoicesAsync();
        Task<TeamModel> GetTeamAsync(long id);
        Task<IEnumerable<TeamStatsViewModel>> GetTeamsAsync(GetTeamsFilterModel filter);
        Task<UserTeamRelationModel> GetUserTeamRelationAsync(long teamId);
        Task<IEnumerable<TeamStatsViewModel>> GetTopTeamsAsync(PagedGetInputModel input);
        Task<IEnumerable<TeamMembershipModel>> GetTopPlayersAsync(long teamId);
        Task<TeamViewerDataModel> GetTeamViewerDataAsync(long teamId);
        Task<UserTeamRelationModel> JoinTeamAsync(JoinTeamRequestInputModel input);
        Task<UserTeamRelationModel> LeaveTeamAsync(long teamId);
        Task ProcessJoinRequestAsync(ProcessJoinTeamRequestInputModel input);
        Task RemoveMemberAsync(long teamId, long membershipId);
        Task UpdateTeamAsync(UpdateTeamInputModel team);
        Task<MultimediaModel> UploadLogoAsync(long teamId);
        Task<ChatRoomViewModel> GetTeamChatAsync(long teamId);
    }
}