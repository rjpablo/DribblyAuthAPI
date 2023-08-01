using Dribbly.Core.Exceptions;
using Dribbly.Core.Models;
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
using Dribbly.Service.Enums;
using Dribbly.Service.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Dribbly.Service.Services
{
    public class TournamentsService : BaseEntityService<TournamentModel>, ITournamentsService
    {
        IAuthContext _context;
        ISecurityUtility _securityUtility;
        private readonly ITournamentsRepository _tournamentsRepository;
        private readonly ITeamsRepository _teamsRepository;
        private readonly INotificationsRepository _notificationsRepo;
        private readonly IFileService _fileService;
        private readonly IIndexedEntitysRepository _indexedEntitysRepository;

        public TournamentsService(IAuthContext context,
            IFileService fileService,
            ISecurityUtility securityUtility
            ) : base(context.Tournaments, context)
        {
            _context = context;
            _securityUtility = securityUtility;
            _tournamentsRepository = new TournamentsRepository(context);
            _teamsRepository = new TeamsRepository(context);
            _notificationsRepo = new NotificationsRepository(context);
            _fileService = fileService;
            _indexedEntitysRepository = new IndexedEntitysRepository(context);
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

        public async Task<bool> IsCurrentUserManagerAsync(long tournamentId)
        {
            var accountId = _securityUtility.GetAccountId().Value;
            return await _context.Tournaments.AnyAsync(t => t.Id == tournamentId && t.AddedById == accountId);
        }

        public async Task<IEnumerable<ChoiceItemModel<long>>> GetTournamentTeamsAsChoicesAsync(long tournamentId, long? stageId)
        {
            var currentAccountId = _securityUtility.GetAccountId();
            List<long> teamIds = await _context.StageTeams.Where(s => (!stageId.HasValue || s.Id == stageId) && s.Stage.TournamentId == tournamentId)
                .Select(s => s.TeamId).ToListAsync();
            return (await _context.Teams.Where(t => teamIds.Contains(t.Id) && t.EntityStatus == EntityStatusEnum.Active)
                .ToListAsync())
                .Select(t => new ChoiceItemModel<long>(t.Name, t.Id, t.Logo?.Url, EntityTypeEnum.Team));
        }

        public async Task<TournamentModel> UpdateTournamentSettingsAsync(UpdateTournamentSettingsModel settings)
        {
            var tournament = await _context.Tournaments.Include(t => t.Games)
                .SingleOrDefaultAsync(t => t.Id == settings.Id);

            if (tournament == null)
            {
                throw new DribblyObjectNotFoundException($"Tournament info not found. Tournament ID: {settings.Id}",
                    friendlyMessage: "Tournament info not found.");
            }

            if (tournament.AddedById != _securityUtility.GetAccountId().Value)
            {
                throw new DribblyForbiddenException("Non-tournament manager tried to update tournament settings.",
                    friendlyMessage: "Only a tournament manager can update these settings.");
            }

            tournament.OverrideSettings(settings);
            if (settings.ApplyToGames)
            {
                foreach (var game in tournament.Games)
                {
                    if (game.Status == GameStatusEnum.WaitingToStart)
                    {
                        game.OverrideSettings(settings);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return await _context.Tournaments.Include(t => t.DefaultCourt.PrimaryPhoto)
                .SingleAsync(t => t.Id == settings.Id);
        }

        public async Task<TournamentViewerModel> GetTournamentViewerAsync(long tournamentId)
        {
            var entity = await _context.Tournaments
                .Include(t => t.Logo)
                .Include(t => t.Games.Select(g => g.Team1.Team.Logo))
                .Include(t => t.Games.Select(g => g.Team2.Team.Logo))
                .Include(t => t.DefaultCourt.PrimaryPhoto)
                .Include(t => t.Teams.Select(tm => tm.Team.Logo))
                .Include(t => t.Stages.Select(s => s.Teams.Select(team => team.Team.Logo)))
                .Include(t => t.Stages.Select(s => s.Games.Select(g => g.Team1.Team.Logo)))
                .Include(t => t.Stages.Select(s => s.Games.Select(g => g.Team2.Team.Logo)))
                .Include(t => t.Stages.Select(s => s.Brackets))
                .Include(t => t.JoinRequests.Select(r => r.Team))
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            if (entity == null)
            {
                throw new DribblyObjectNotFoundException($"Tournament info not found. Tournament ID: {tournamentId}",
                           friendlyMessage: "Tournament info not found. The tournament info may have been deleted.");
            }
            else
            {
                entity.Games = entity.Games.Where(g => g.EntityStatus != Enums.EntityStatusEnum.Deleted).ToList();
                return new TournamentViewerModel(entity);
            }
        }

        public async Task<IEnumerable<TeamStatsViewModel>> GetTopTeamsAsync(GetTournamentTeamsInputModel input)
        {
            return (await _context.TournamentTeams.Include(s => s.Team.Logo)
                .Where(t => t.TournamentId == input.TournamentId)
                .OrderByDescending(s => s.OverallScore)
                .ThenBy(s => s.Team.Name)
                .Skip(input.PageSize * (input.Page - 1))
                .ToListAsync()).Select(s => new TeamStatsViewModel(s));
        }

        public async Task<PhotoModel> UpdateLogoAsync(long tournamentId)
        {
            TournamentModel tournament = await GetTournamentByIdAsync(tournamentId);
            if (tournament == null)
            {
                throw new DribblyObjectNotFoundException($"Tournament info not found. Tournament ID: {tournamentId}",
                    friendlyMessage: "Tournament info not found.");
            }

            var accountId = _securityUtility.GetAccountId().Value;

            if (tournament.AddedById != accountId)
            {
                throw new DribblyForbiddenException("Non-tournament manager tried to update tournament settings.",
                    friendlyMessage: "Only a tournament manager can update these settings.");
            }

            HttpFileCollection files = HttpContext.Current.Request.Files;
            if (files.Count == 0)
            {
                throw new DribblyInvalidOperationException("No files to upload",
                    friendlyMessage: "The request did not contain the file to be uploaded");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    PhotoModel photo = await AddPhoto(tournament, files[0]);
                    tournament.LogoId = photo.Id;
                    Update(tournament);
                    await _context.SaveChangesAsync();
                    await _indexedEntitysRepository.SetIconUrl(_context, tournament, photo.Url);
                    //TODO: log user activity
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

        private async Task<PhotoModel> AddPhoto(TournamentModel tournament, HttpPostedFile file)
        {
            var accountId = _securityUtility.GetAccountId().Value;
            string uploadPath = _fileService.Upload(file, $"{accountId}/tournamentPhotos/");

            PhotoModel photo = new PhotoModel
            {
                Url = uploadPath,
                UploadedById = accountId,
                DateAdded = DateTime.UtcNow
            };
            _context.Photos.Add(photo);
            await _context.SaveChangesAsync();

            _context.TournamentPhotos.Add(new TournamentPhotoModel
            {
                PhotoId = photo.Id,
                TournamentId = tournament.Id,
                DateAdded = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            return photo;
        }

        #region Stages and Brackets

        public async Task<TournamentStageModel> AddTournamentStageAsync(AddTournamentStageInputModel input)
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

                    return await _context.TournamentStages
                        .Include(s => s.Teams.Select(team => team.Team.Logo))
                        .Include(s => s.Games.Select(t => t.Team1.Team.Logo))
                        .Include(s => s.Games.Select(t => t.Team2.Team.Logo))
                        .SingleAsync(s => s.Id == stage.Id);
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
            return await _context.TournamentStages.Include(s => s.Brackets).Include(s => s.Teams.Select(t => t.Team.Logo))
                .Where(s => s.TournamentId == tournamentId).ToListAsync();
        }

        public async Task SetTeamBracket(long teamId, long stageId, long? bracketId)
        {
            var team = await _context.StageTeams.SingleOrDefaultAsync(t => t.TeamId == teamId && t.StageId == stageId);
            team.BracketId = bracketId;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStageAsync(long stageId)
        {
            var stage = await _context.TournamentStages
                .Include(s => s.Games).Include(s => s.Teams).Include(s => s.Tournament)
                .SingleOrDefaultAsync(b => b.Id == stageId);

            if (stage == null)
            {
                return;
            }

            if (stage.Tournament.AddedById != _securityUtility.GetAccountId().Value)
            {
                throw new DribblyForbiddenException("Non-tournament manager tried to delete stage.",
                    friendlyMessage: "Only a tournament manager can delete a stage");
            }

            if (stage.Games.Any(g => g.Status != GameStatusEnum.Cancelled && g.Status != GameStatusEnum.Deleted))
            {
                throw new DribblyInvalidOperationException("Tried to delete stage with active games",
                    friendlyMessage: "Cannot delete stage becuase it has active games. " +
                    "Please cancel all games in the stage before deleting the stage");
            }

            _context.TournamentStages.Remove(stage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStageBracketAsync(long bracketId)
        {
            var bracket = await _context.StageBrackets.SingleOrDefaultAsync(b => b.Id == bracketId);

            if (bracket == null)
            {
                return;
            }

            var bracketTeams = await _context.StageTeams.Where(t => t.BracketId == bracketId)
                .ToListAsync();
            foreach (var team in bracketTeams)
            {
                team.BracketId = null;
            }

            _context.StageBrackets.Remove(bracket);
            await _context.SaveChangesAsync();
        }

        public async Task RenameStageAsync(long stageId, string name)
        {
            var stage = await _context.TournamentStages.Include(s => s.Brackets)
                .SingleOrDefaultAsync(s => s.Id == stageId);

            if (stage == null)
            {
                throw new DribblyObjectNotFoundException($"Stage info not found. Stage ID: {stageId}",
                    friendlyMessage: "Stage info could not be found.");
            }
            else if (_context.TournamentStages.Any(s => s.TournamentId == stage.TournamentId && s.Id != stageId &&
             s.Name.ToLower() == name.ToLower()))
            {
                throw new DribblyInvalidOperationException("Stage name not available",
                    friendlyMessage: $"There is already a stage named {name}");
            }

            stage.Name = name;
            await _context.SaveChangesAsync();
        }

        public async Task<StageBracketModel> AddStageBracketAsync(string bracketName, long stageId)
        {
            var stage = await _context.TournamentStages.Include(s => s.Brackets)
                .SingleOrDefaultAsync(s => s.Id == stageId);

            if (stage == null)
            {
                throw new DribblyObjectNotFoundException($"Stage info not found. Stage ID: {stageId}",
                    friendlyMessage: "Stage info could not be found.");
            }

            if (stage.Brackets.Any(b => b.Name.ToLower() == bracketName.ToLower()))
            {
                throw new DribblyInvalidOperationException($"Duplicate bracket name: {bracketName}",
                    friendlyMessage: $"There's already a bracket named {bracketName}");
            }

            var bracket = new StageBracketModel
            {
                StageId = stageId,
                Name = bracketName,
                DateAdded = DateTime.UtcNow,
                AddedById = _securityUtility.GetAccountId().Value
            };
            _context.StageBrackets.Add(bracket);

            await _context.SaveChangesAsync();
            return _context.StageBrackets.Single(b => b.Id == bracket.Id);
        }

        #endregion

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
                var tournamentTeam = await _context.TournamentTeams
                    .SingleAsync(t => t.TeamId == request.TeamId && t.TournamentId == request.TournamentId);

                if (tournamentTeam == null)
                {
                    tournamentTeam = new TournamentTeamModel
                    {
                        TeamId = request.TeamId,
                        TournamentId = request.TournamentId,
                        DateAdded = DateTime.UtcNow
                    };
                    _context.TournamentTeams.Add(tournamentTeam);
                    _ = AddJoinTournamentNotification(request, NotificationTypeEnum.JoinTournamentRequestApproved);
                    await _context.SaveChangesAsync();
                    tournamentTeam = await _context.TournamentTeams
                        .SingleAsync(t => t.TeamId == request.TeamId && t.TournamentId == tournamentTeam.Id);
                }

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

        #region Helper Methods
        private async Task<TournamentModel> GetTournamentByIdAsync(long tournamentId)
        {
            return await _context.Tournaments.SingleOrDefaultAsync(t => t.Id == tournamentId);
        }
        #endregion
    }

    public interface ITournamentsService
    {
        Task<TournamentModel> AddTournamentAsync(TournamentModel season);
        Task<TournamentViewerModel> GetTournamentViewerAsync(long tournamentId);
        Task<IEnumerable<TeamStatsViewModel>> GetTopTeamsAsync(GetTournamentTeamsInputModel input);
        Task<PhotoModel> UpdateLogoAsync(long tournamentId);
        Task<IEnumerable<TournamentModel>> GetNewAsync(GetTournamentsInputModel input);
        Task RemoveTournamentTeamAsync(long tournamentId, long teamId);
        Task<bool> IsCurrentUserManagerAsync(long tournamentId);
        Task<TournamentModel> UpdateTournamentSettingsAsync(UpdateTournamentSettingsModel settings);
        Task<IEnumerable<ChoiceItemModel<long>>> GetTournamentTeamsAsChoicesAsync(long tournamentId, long? stageId);

        #region Stages and Brackets
        Task<TournamentStageModel> AddTournamentStageAsync(AddTournamentStageInputModel input);
        Task<IEnumerable<TournamentStageModel>> GetTournamentStagesAsync(long tournamentId);
        Task<TournamentStageModel> SetStageTeamsAsync(SetStageTeamsInputModel input);
        Task DeleteStageAsync(long stageId);
        Task SetTeamBracket(long teamId, long stageId, long? bracketId);
        Task<StageBracketModel> AddStageBracketAsync(string bracketName, long stageId);
        Task RenameStageAsync(long stageId, string name);
        Task DeleteStageBracketAsync(long bracketId);
        #endregion

        #region Join Requests
        Task JoinTournamentAsync(long tournamentId, long teamId);
        Task<TeamStatsViewModel> ProcessJoinRequestAsync(long requestId, bool shouldApprove);
        #endregion
    }
}