using Dribbly.Model;
using Dribbly.Model.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
{
    public class NotificationsRepository : BaseRepository<NotificationModel>, INotificationsRepository
    {
        AuthContext _context;

        public NotificationsRepository(IAuthContext context) : base(context.Notifications)
        {
            _context = new AuthContext();
        }

        public async Task TryAddAsync(object notification)
        {
            // Creating notifications should not prevent transactions
            try
            {
                switch (((NotificationModel)notification).Type)
                {
                    case NotificationTypeEnum.NewGameForOwner:
                    case NotificationTypeEnum.NewGameForBooker:
                        _context.NewGameNotifications.Add((NewGameNotificationModel)notification);
                        break;
                    case NotificationTypeEnum.JoinTeamRequest:
                    case NotificationTypeEnum.JoinTeamRequestApproved:
                        _context.JoinTeamRequestNotifications.Add((JoinTeamRequestNotificationModel)notification);
                        break;
                }
                await _context.SaveChangesAsync();
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
        Task TryAddAsync(object notification);
        IQueryable<NotificationModel> GetUnviewed(long? userId, DateTime? afterDate);
    }
}