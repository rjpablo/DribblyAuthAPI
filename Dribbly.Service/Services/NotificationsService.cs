using Dribbly.Authentication.Models.Auth;
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
            return await _notificationsRepo.GetUnviewed(_securityUtility.GetUserId(), afterDate)
                .OrderByDescending(n => n.DateAdded).ToListAsync();
        }

        #region Get Unviewed Count
        public async Task<UnviewedCountModel> GetUnviewedCountAsync(long? userId)
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
            long? currentUserId = _securityUtility.GetUserId();
            IEnumerable<NotificationModel> notifications = _context.Notifications
                .Where(n => currentUserId.HasValue && n.ForUserId == currentUserId && (n.DateAdded < beforeDate || beforeDate == null))
                .OrderByDescending(n => n.DateAdded).Take(getCount);

            return await GetDetailedNotificationsAsync(notifications);
        }

        public async Task<GetNewNotificationsResultModel> GetNewNoficationsAsync(DateTime afterDate)
        {
            long? currentUserId = _securityUtility.GetUserId();
            IEnumerable<NotificationModel> notifications = _context.Notifications
                .Where(n => currentUserId.HasValue && n.ForUserId == currentUserId && (n.DateAdded > afterDate));

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
            List<NotificationModel> resultWithDetails = new List<NotificationModel>();

            // Booking Booked - For Court owner
            IEnumerable<NewBookingNotificationModel> newBookingNotifications = await GetNewBookingNotificationsAsync
                (notifications.Where(n => n.Type == NotificationTypeEnum.NewBookingForBooker || n.Type == NotificationTypeEnum.NewBookingForOwner)
                .Select(n => n.Id).ToArray());

            var joinTeamRequestNotifications = await GetJoinTeamRequestNotificationsAsync
                (notifications.Where(n => n.Type == NotificationTypeEnum.JoinTeamRequest)
                .Select(n => n.Id).ToArray());

            resultWithDetails.AddRange(joinTeamRequestNotifications);
            resultWithDetails.AddRange(newBookingNotifications);

            return resultWithDetails.OrderByDescending(n=>n.DateAdded);
        }

        private async Task<IEnumerable<NewBookingNotificationModel>> GetNewBookingNotificationsAsync(long[] NotificationIds)
        {
            if (NotificationIds.Length == 0) return await Task.FromResult<IEnumerable<NewBookingNotificationModel>>
                    (new List<NewBookingNotificationModel>());

            return await _context.NewBookingNotifications
                .Where(n => NotificationIds.Contains(n.Id))
                .Include(n => n.Booking).Include(n => n.Booking.Court).Include(n => n.BookedBy)
                .Select(n => new NewBookingNotificationDto
                {
                    Id = n.Id,
                    DateAdded = n.DateAdded,
                    ForUserId = n.ForUserId,
                    IsViewed = n.IsViewed,
                    Type = n.Type,
                    Booking = n.Booking,
                    BookedById = n.BookedById,
                    BookedByName = n.BookedBy.UserName,
                    CourtName = n.Booking.Court.Name
                })
                .ToListAsync();
        }

        private async Task<IEnumerable<JoinTeamRequestNotificationViewModel>> GetJoinTeamRequestNotificationsAsync(long[] NotificationIds)
        {
            if (NotificationIds.Length == 0) return await Task.FromResult<IEnumerable<JoinTeamRequestNotificationViewModel>>
                    (new List<JoinTeamRequestNotificationViewModel>());

            return await _context.JoinTeamRequestNotifications
                .Where(n => NotificationIds.Contains(n.Id)).Include(n => n.Request)
                .Include(n => n.Request.Team).Include(n => n.Request.Member).Include(n => n.Request.Member.User)
                .Select(n => new JoinTeamRequestNotificationViewModel
                {
                    Id = n.Id,
                    DateAdded = n.DateAdded,
                    ForUserId = n.ForUserId,
                    IsViewed = n.IsViewed,
                    Type = n.Type,
                    TeamId = n.Request.TeamId, 
                    TeamName = n.Request.Team.Name,
                    RequestorId = n.Request.MemberAccountId,
                    RequestorName = n.Request.Member.User.UserName
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