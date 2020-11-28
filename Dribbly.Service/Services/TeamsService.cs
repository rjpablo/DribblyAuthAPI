using Dribbly.Core.Exceptions;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Teams;
using Dribbly.Model.Notifications;
using Dribbly.Service.Enums;
using Dribbly.Service.Repositories;
using Dribbly.Service.Services.Shared;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public interface ITeamsService
    {
        Task<TeamModel> AddTeamAsync(TeamModel team);
        IEnumerable<TeamModel> GetAll();
        Task<TeamModel> GetTeam(long id);
        Task UpdateTeamAsync(TeamModel team);
        Task<TeamViewerDataModel> GetTeamViewerDataAsync(long teamId);
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
                Team = await GetTeam(teamId)
            };
        }

        public async Task<TeamModel> GetTeam(long id)
        {
            TeamModel team = await _dbSet.Where(g => g.Id == id).Include(g => g.HomeCourt).Include(t=>t.Logo)
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

        public async Task<TeamModel> AddTeamAsync(TeamModel team)
        {
            var currentUserId = _securityUtility.GetUserId();
            team.AddedById = currentUserId.Value;
            team.Status = EntityStatusEnum.Active;
            Add(team);
            _context.SaveChanges();
            await _indexedEntitysRepository.Add(_context, team);
            await _commonService.AddUserTeamActivity(UserActivityTypeEnum.AddTeam, team.Id);

            return team;
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