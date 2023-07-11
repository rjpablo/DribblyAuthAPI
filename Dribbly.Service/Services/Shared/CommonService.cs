using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Courts;
using Dribbly.Model.Shared;
using Dribbly.Model.UserActivities;
using Dribbly.Service.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;

namespace Dribbly.Service.Services.Shared
{
    #region Interface
    public interface ICommonService
    {
        #region User Activities
        Task AddUserPostActivity(UserActivityTypeEnum activityType, long postId);
        Task AddUserAccountActivity(UserActivityTypeEnum activityType, long accountId);
        Task AddAccountPhotoActivitiesAsync(UserActivityTypeEnum activityType, long accountId, params PhotoModel[] photos);
        Task AddAccountVideoActivitiesAsync(UserActivityTypeEnum activityType, long accountId, params VideoModel[] photos);
        Task AddUserContactActivity(UserActivityTypeEnum activityType, long? contactId, string contactNo);
        // COURTS
        Task AddUserCourtActivity(UserActivityTypeEnum activityType, long courtId);
        Task AddCourtVideosActivity(UserActivityTypeEnum activityType, long courtId, params VideoModel[] videos);
        Task AddCourtPhotosActivity(UserActivityTypeEnum activityType, long courtId, params PhotoModel[] photos);
        // GAMES
        Task AddUserGameActivity(UserActivityTypeEnum activityType, long gameId);
        //TEAMS
        Task AddUserTeamActivity(UserActivityTypeEnum activityType, long teamId);
        Task AddTeamPhotoActivitiesAsync(UserActivityTypeEnum activityType, long teamId, params PhotoModel[] photos);
        Task AddUserJoinTeamRequestActivity(UserActivityTypeEnum activityType, long requestId);
        #endregion

        #region Search Suggestions
        Task<IEnumerable<ChoiceItemModel<long>>> GetTypeAheadSuggestionsAsync
            (GetTypeAheadSuggestionsInputModel input);
        Task<ChoiceItemModel<long>> GetChoiceItemModelAsync
            (long? id, EntityTypeEnum entityType);
        #endregion

        Task<IndexedEntityModel> GetIndexedEntity
            (long? id, EntityTypeEnum entityType);
        Task<long?> GetAccountId();
        string TryGetRequestData(HttpRequest request);
    }
    #endregion

    public class CommonService : ICommonService
    {
        IAuthContext _context;
        ISecurityUtility _securityUtility;
        public CommonService(IAuthContext context,
            ISecurityUtility securityUtility)
        {
            _context = context;
            _securityUtility = securityUtility;
        }

        #region User Activites - Account

        public async Task LogUserActivity(UserActivityModel log)
        {
            log.UserId = (await GetAccountId()).Value;
            log.DateAdded = DateTime.UtcNow;
            _context.UserActivities.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task AddAccountPhotoActivitiesAsync(UserActivityTypeEnum activityType, long accountId, params PhotoModel[] photos)
        {
            List<UserActivityModel> activities = new List<UserActivityModel>();
            foreach (var photo in photos)
            {
                var activity = new AccountPhotoActivityModel
                {
                    Type = activityType,
                    AccountId = accountId,
                    PhotoId = photo.Id
                };

                activities.Add(activity);
            }
            await AddActivitiesAsync(activities);
        }

        public async Task AddAccountVideoActivitiesAsync(UserActivityTypeEnum activityType, long accountId, params VideoModel[] photos)
        {
            List<UserActivityModel> activities = new List<UserActivityModel>();
            foreach (var photo in photos)
            {
                var activity = new AccountVideoActivityModel
                {
                    Type = activityType,
                    AccountId = accountId,
                    VideoId = photo.Id
                };

                activities.Add(activity);
            }
            await AddActivitiesAsync(activities);
        }

        public async Task AddUserAccountActivity(UserActivityTypeEnum activityType, long accountId)
        {
            var activity = new UserAccountActivityModel
            {
                Type = activityType,
                AccountId = accountId
            };

            await AddActivityAsync(activity);
        }

        #endregion

        #region User Activities - Posts

        public async Task AddUserPostActivity(UserActivityTypeEnum activityType, long postId)
        {
            var activity = new UserPostActivityModel
            {
                Type = activityType,
                PostId = postId
            };

            await AddActivityAsync(activity);
        }

        #endregion

        #region User Activities - Games

        public async Task AddUserGameActivity(UserActivityTypeEnum activityType, long gameId)
        {
            var activity = new UserGameActivityModel
            {
                Type = activityType,
                GameId = gameId
            };

            await AddActivityAsync(activity);
        }

        #endregion

        #region User Activities - Teams

        public async Task AddUserTeamActivity(UserActivityTypeEnum activityType, long teamId)
        {
            var activity = new UserTeamActivityModel
            {
                Type = activityType,
                TeamId = teamId
            };

            await AddActivityAsync(activity);
        }

        public async Task AddUserJoinTeamRequestActivity(UserActivityTypeEnum activityType, long requestId)
        {
            var activity = new UserJoinTeamRequestActivityModel
            {
                Type = activityType,
                RequestId = requestId
            };

            await AddActivityAsync(activity);
        }

        public async Task AddTeamPhotoActivitiesAsync(UserActivityTypeEnum activityType, long teamId, params PhotoModel[] photos)
        {
            List<UserActivityModel> activities = new List<UserActivityModel>();
            foreach (var photo in photos)
            {
                var activity = new TeamPhotoActivityModel
                {
                    Type = activityType,
                    TeamId = teamId,
                    PhotoId = photo.Id
                };

                activities.Add(activity);
            }
            await AddActivitiesAsync(activities);
        }

        #endregion

        #region User Activities - Courts
        public async Task AddUserCourtActivity(UserActivityTypeEnum activityType, long courtId)
        {
            var activity = new UserCourtActivityModel
            {
                Type = activityType,
                CourtId = courtId
            };

            await AddActivityAsync(activity);
        }

        public async Task AddCourtPhotosActivity(UserActivityTypeEnum activityType, long courtId, params PhotoModel[] photos)
        {
            List<UserActivityModel> activities = new List<UserActivityModel>();
            foreach (var photo in photos)
            {
                var activity = new CourtPhotoActivityModel
                {
                    Type = activityType,
                    CourtId = courtId,
                    PhotoId = photo.Id
                };

                activities.Add(activity);
            }
            await AddActivitiesAsync(activities);
        }

        public async Task AddCourtVideosActivity(UserActivityTypeEnum activityType, long courtId, params VideoModel[] videos)
        {
            List<UserActivityModel> activities = new List<UserActivityModel>();
            foreach (var video in videos)
            {
                var activity = new CourtVideoActivityModel
                {
                    Type = activityType,
                    CourtId = courtId,
                    VideoId = video.Id
                };

                activities.Add(activity);
            }
            await AddActivitiesAsync(activities);
        }

        #endregion

        #region User Activities - Contacts

        public async Task AddUserContactActivity(UserActivityTypeEnum activityType, long? contactId, string contactNo)
        {
            var activity = new UserContactActivityModel
            {
                Type = activityType,
                ContactId = contactId,
                ContactNo = contactNo
            };
            await AddActivityAsync(activity);
        }

        #endregion

        #region User Activities - Common


        private async Task AddActivityAsync(UserActivityModel activity)
        {
            var userId = await GetAccountId();
            if (userId.HasValue)
            {
                activity.UserId = userId.Value;
                activity.DateAdded = DateTime.UtcNow;
                AddActivity(activity);
                await _context.SaveChangesAsync();
            }
        }

        private async Task AddActivitiesAsync(List<UserActivityModel> activities)
        {
            var accountId = await GetAccountId();
            if (accountId.HasValue)
            {
                var now = DateTime.UtcNow;
                foreach (var activity in activities)
                {
                    activity.UserId = accountId.Value;
                    activity.DateAdded = now;
                    AddActivity(activity);
                }
                await _context.SaveChangesAsync();
            }
        }

        private void AddActivity(UserActivityModel activity)
        {
            // POSTS
            if (activity is UserPostActivityModel)
            {
                _context.UserPostActivities.Add((UserPostActivityModel)activity);
            }
            // ACCOUNTS
            else if (activity is UserAccountActivityModel)
            {
                if (activity is AccountPhotoActivityModel)
                {
                    _context.AccountPhotoActivities.Add((AccountPhotoActivityModel)activity);
                }
                else if (activity is AccountVideoActivityModel)
                {
                    _context.AccountVideoActivities.Add((AccountVideoActivityModel)activity);
                }
                else
                {
                    _context.UserAccountActivities.Add((UserAccountActivityModel)activity);
                }
            }
            // CONTACTS
            else if (activity is UserContactActivityModel)
            {
                _context.UserContactActivities.Add((UserContactActivityModel)activity);
            }
            // COURTS
            else if (activity is UserCourtActivityModel)
            {
                _context.UserCourtActivities.Add((UserCourtActivityModel)activity);
            }
            else if (activity is CourtPhotoActivityModel)
            {
                _context.CourtPhotoActivities.Add((CourtPhotoActivityModel)activity);
            }
            else if (activity is CourtVideoActivityModel)
            {
                _context.CourtVideoActivities.Add((CourtVideoActivityModel)activity);
            }
            // GAMES
            else if (activity is UserGameActivityModel)
            {
                _context.UserGameActivities.Add((UserGameActivityModel)activity);
            }
            // TEAMS
            else if (activity is TeamPhotoActivityModel)
            {
                _context.TeamPhotoActivities.Add((TeamPhotoActivityModel)activity);
            }
            else if (activity is UserTeamActivityModel)
            {
                _context.UserTeamActivities.Add((UserTeamActivityModel)activity);
            }
            else if (activity is UserJoinTeamRequestActivityModel)
            {
                _context.UserJoinTeamRequestActivities.Add((UserJoinTeamRequestActivityModel)activity);
            }
            else
            {
                //TODO: Log warning - activity not recorded
            }
        }
        #endregion

        #region Search Suggestions

        public async Task<IEnumerable<ChoiceItemModel<long>>> GetTypeAheadSuggestionsAsync
            (GetTypeAheadSuggestionsInputModel input)
        {
            var query = _context.IndexedEntities.Where(i => (!input.EntityTypes.Any() || input.EntityTypes.Contains(i.EntityType)) &&
            i.Name.Contains(input.Keyword) && i.EntityStatus == EntityStatusEnum.Active);

            if (input.MaxCount.HasValue)
            {
                query = query.Take(input.MaxCount.Value);
            }

            var result = await query.ToListAsync();
            return result.Select(i => i.ToChoiceItemModel());
        }

        public async Task<ChoiceItemModel<long>> GetChoiceItemModelAsync
            (long? id, EntityTypeEnum entityType)
        {
            return (await GetIndexedEntity(id, entityType))?.ToChoiceItemModel();
        }

        public async Task<IndexedEntityModel> GetIndexedEntity
            (long? id, EntityTypeEnum entityType)
        {
            return await _context.IndexedEntities.SingleOrDefaultAsync
                (i => id != null && i.Id == id && i.EntityType == entityType);
        }

        #endregion

        #region Helpers

        public async Task<long?> GetAccountId()
        {
            var stringUserId = ClaimsPrincipal.Current.Claims.ToList()
                .SingleOrDefault(c => c.Type == "userId")?.Value;
            if (string.IsNullOrEmpty(stringUserId))
            {
                return null;
            }
            var userId = long.Parse(stringUserId);
            var accountId = (await _context.Accounts.SingleOrDefaultAsync(a => a.IdentityUserId == userId))?.Id;

            return accountId;
        }

        public string TryGetRequestData(HttpRequest request)
        {
            try
            {
                using (StreamReader stream = new StreamReader(request.InputStream))
                {
                    return stream.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return "(failed to read)";
            }
        }

        #endregion

    }
}