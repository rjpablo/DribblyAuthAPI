using Dribbly.Model;
using Dribbly.Model.Notifications;
using Dribbly.Service.Hubs;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
{
    public class NotificationsRepository : BaseRepository<NotificationModel>, INotificationsRepository
    {
        IAuthContext _context;
        private static IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationsHub>();

        public NotificationsRepository(IAuthContext context) : base(context.Notifications)
        {
            _context = context;
        }

        public async Task TryAddAsync(INotificationModel notification)
        {
            // Creating notifications should not prevent transactions
            try
            {
                notification.DateAdded = DateTime.UtcNow;
                switch ((notification).Type)
                {
                    // NewBookingNotification and UpdateGameNotification
                    case NotificationTypeEnum.NewGameForBooker:
                    case NotificationTypeEnum.NewGameForOwner:
                    case NotificationTypeEnum.JoinTeamRequest:
                    case NotificationTypeEnum.JoinTeamRequestApproved:
                    case NotificationTypeEnum.NewJoinTournamentRequest:
                    case NotificationTypeEnum.JoinTournamentRequestApproved:
                    case NotificationTypeEnum.JoinTournamentRequestRejected:
                    case NotificationTypeEnum.TournamentTeamRemoved:
                    case NotificationTypeEnum.AssignedAsTimekeeper:
                    case NotificationTypeEnum.JoinEventRequest:
                    case NotificationTypeEnum.JoinEventRequestApproved:
                    case NotificationTypeEnum.JoinGroupRequest:
                    case NotificationTypeEnum.JoinGroupRequestApproved:
                    case NotificationTypeEnum.PostReceivedReaction:
                        _context.Notifications.Add((NotificationModel)notification);
                        break;
                }
                await _context.SaveChangesAsync();
                _hubContext.Clients.Group(notification.ForUserId.ToString())
                    .receiveNotification(notification);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public IQueryable<NotificationModel> GetUnviewed(long? userId, DateTime? afterDate)
        {
            return _dbSet.Where(n => userId.HasValue && n.ForUserId == userId && !n.IsViewed &&
            (afterDate == null || n.DateAdded > afterDate));
        }

    }

    public interface INotificationsRepository
    {
        Task TryAddAsync(INotificationModel notification);
        IQueryable<NotificationModel> GetUnviewed(long? userId, DateTime? afterDate);
    }
}