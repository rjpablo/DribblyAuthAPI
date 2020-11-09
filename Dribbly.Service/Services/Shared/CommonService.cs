﻿using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Courts;
using Dribbly.Model.Shared;
using Dribbly.Model.UserActivities;
using Dribbly.Service.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Dribbly.Service.Services.Shared
{
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
            log.UserId = GetUserId();
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

        #region User Activities - Common


        private async Task AddActivityAsync(UserActivityModel activity)
        {
            activity.UserId = GetUserId();
            activity.DateAdded = DateTime.UtcNow;
            AddActivity(activity);
            await _context.SaveChangesAsync();
        }

        private async Task AddActivitiesAsync(List<UserActivityModel> activities)
        {
            var userId = GetUserId();
            var now = DateTime.UtcNow;
            foreach (var activity in activities)
            {
                activity.UserId = userId;
                activity.DateAdded = now;
                AddActivity(activity);
            }
            await _context.SaveChangesAsync();
        }

        private void AddActivity(UserActivityModel activity)
        {
            if (activity is UserPostActivityModel)
            {
                _context.UserPostActivities.Add((UserPostActivityModel)activity);
            }
            else if (activity is UserAccountActivityModel)
            {
                _context.UserAccountActivities.Add((UserAccountActivityModel)activity);
            }
            else if (activity is AccountPhotoActivityModel)
            {
                _context.AccountPhotoActivities.Add((AccountPhotoActivityModel)activity);
            }
            else if (activity is AccountVideoActivityModel)
            {
                _context.AccountVideoActivities.Add((AccountVideoActivityModel)activity);
            }
            else
            {
                //TODO: Log warning - activity not recorded
            }
        }
        #endregion

        #region Helpers

        public string GetUserId()
        {
            var userId = ClaimsPrincipal.Current.Claims.ToList()
                .SingleOrDefault(c => c.Type == "userId")?.Value;
            return userId;
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