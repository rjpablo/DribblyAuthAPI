﻿using Dribbly.Authentication.Models.Auth;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Account;
using Dribbly.Model.Notifications;
using Dribbly.Service.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public class NotificationsService : BaseEntityService<NotificationModel>, INotificationsService
    {
        IAuthContext _context;
        ISecurityUtility _securityUtility;
        INotificationsRepository _notificationsRepo;

        public NotificationsService(IAuthContext context,
            ISecurityUtility securityUtility,
            INotificationsRepository notificationsRepo) : base(context.Notifications)
        {
            _context = context;
            _securityUtility = securityUtility;
            _notificationsRepo = notificationsRepo;
        }

        public async Task<IEnumerable<NotificationModel>> GetUnviewedAsync(DateTime? afterDate)
        {
            string currentUserId = _securityUtility.GetUserId();
            return await _notificationsRepo.GetUnviewed(currentUserId, afterDate)
                .OrderByDescending(n => n.DateAdded).ToListAsync();
        }

        #region Get Unviewed Count
        public async Task<UnviewedCountModel> GetUnviewedCountAsync(string userId)
        {
            return new UnviewedCountModel
            {
                Count = await _notificationsRepo.GetUnviewed(userId, null).CountAsync(),
                AsOf = DateTime.UtcNow
            };
        }

        public async Task<UnviewedCountModel> GetUnviewedCountAsync()
        {
            return await GetUnviewedCountAsync(_securityUtility.GetUserId());
        }
        #endregion

        public async Task<UnviewedCountModel> SetIsViewedAsync(long notificationId, bool isViewed)
        {
            NotificationModel notification = SingleOrDefault(n => n.Id == notificationId);
            notification.IsViewed = isViewed;
            Update(notification);
            await _context.SaveChangesAsync();
            return await GetUnviewedCountAsync(_securityUtility.GetUserId());
        }

        public async Task<IEnumerable<object>> GetNoficationDetailsAsync(DateTime? beforeDate, int getCount = 10)
        {
            string currentUserId = _securityUtility.GetUserId();
            IEnumerable<NotificationModel> notifications = _context.Notifications
                .Where(n => n.ForUserId == currentUserId && (n.DateAdded < beforeDate || beforeDate == null))
                .OrderByDescending(n => n.DateAdded).Take(getCount);

            return await GetDetailedNotificationsAsync(notifications);
        }

        public async Task<GetNewNotificationsResultModel> GetNewNoficationsAsync(DateTime afterDate)
        {
            string currentUserId = _securityUtility.GetUserId();
            IEnumerable<NotificationModel> notifications = _context.Notifications
                .Where(n => n.ForUserId == currentUserId && (n.DateAdded > afterDate));

            var defaultNotifications = await GetDetailedNotificationsAsync(notifications);

            if(defaultNotifications.Count() > 0)
            {
                return new GetNewNotificationsResultModel
                {
                    Notifications = defaultNotifications,
                    UnviewedCount = await GetUnviewedCountAsync(currentUserId)
                };
            }
            else
            {
                return null;
            }
        }

        private async Task<IEnumerable<object>> GetDetailedNotificationsAsync(IEnumerable<NotificationModel> notifications)
        {
            List<object> resultWithDetails = new List<object>();

            // Game Booked - For Court owner
            IEnumerable<GameBookedNotificationViewModel> bookedGameForOwner = await GameBookedNotificationsAsync
                (notifications.Where(n => n.Type == NotificationTypeEnum.GameBookedForBooker || n.Type == NotificationTypeEnum.GameBookedForOwner)
                .Select(n => n.Id).ToArray());

            resultWithDetails.AddRange(bookedGameForOwner);

            return resultWithDetails;
        }

        private async Task<IEnumerable<GameBookedNotificationViewModel>> GameBookedNotificationsAsync(long[] NotificationIds)
        {
            if (NotificationIds.Length == 0) return await Task.FromResult<IEnumerable<GameBookedNotificationViewModel>>
                    (new List<GameBookedNotificationViewModel>());

            return await _context.GameBookedNotifications
                .Where(n => NotificationIds.Contains(n.Id))
                .Include(n => n.Game).Include(n => n.Game.Court).Include(n => n.BookedBy)
                .Select(n => new GameBookedNotificationViewModel
                {
                    Id = n.Id,
                    DateAdded = n.DateAdded,
                    ForUserId = n.ForUserId,
                    IsViewed = n.IsViewed,
                    Type = n.Type,
                    GameId = n.GameId,
                    BookedById = n.BookedById,
                    BookedByName = n.BookedBy.UserName,
                    CourtName = n.Game.Court.Name
                })
                .ToListAsync();
        }
    }

    public interface INotificationsService
    {
        Task<IEnumerable<NotificationModel>> GetUnviewedAsync(DateTime? afterDate);

        Task<UnviewedCountModel> GetUnviewedCountAsync();

        Task<GetNewNotificationsResultModel> GetNewNoficationsAsync(DateTime afterDate);

        Task<IEnumerable<object>> GetNoficationDetailsAsync(DateTime? beforeDate, int getCount = 10);

        Task<UnviewedCountModel> SetIsViewedAsync(long notificationId, bool isViewed);
    }
}