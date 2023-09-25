using Dribbly.Authentication.Models.Auth;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Account;
using Dribbly.Model.Notifications;
using Dribbly.Service.Hubs;
using Dribbly.Service.Repositories;
using Microsoft.AspNet.SignalR;
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
        IAccountRepository _accountsRepo;

        public NotificationsService(IAuthContext context,
            ISecurityUtility securityUtility,
            INotificationsRepository notificationsRepo,
            IAccountRepository accountsRepo) : base(context.Notifications, context)
        {
            _context = context;
            _securityUtility = securityUtility;
            _notificationsRepo = notificationsRepo;
            _accountsRepo = accountsRepo;
        }

        public async Task<IEnumerable<NotificationModel>> GetUnviewedAsync(DateTime? afterDate)
        {
            return await _notificationsRepo.GetUnviewed(_securityUtility.GetAccountId().Value, afterDate)
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
            return await GetUnviewedCountAsync(_securityUtility.GetAccountId().Value);
        }
        #endregion

        public async Task<UnviewedCountModel> SetIsViewedAsync(long notificationId, bool isViewed)
        {
            NotificationModel notification = SingleOrDefault(n => n.Id == notificationId);
            notification.IsViewed = isViewed;
            Update(notification);
            await _context.SaveChangesAsync();
            return await GetUnviewedCountAsync(_securityUtility.GetAccountId().Value);
        }

        public async Task<IEnumerable<object>> GetNoficationDetailsAsync(DateTime? beforeDate, int getCount = 10)
        {
            long? currentAccountId = _securityUtility.GetAccountId().Value;
            IEnumerable<NotificationModel> notifications = await _context.Notifications
                .Where(n => currentAccountId.HasValue && n.ForUserId == currentAccountId && (beforeDate == null || n.DateAdded < beforeDate))
                .OrderByDescending(n => n.DateAdded).Take(getCount)
                .ToListAsync();

            return await GetDetailedNotificationsAsync(notifications);
        }

        public async Task<GetNewNotificationsResultModel> GetNewNoficationsAsync(DateTime afterDate)
        {
            var accountId = _securityUtility.GetAccountId().Value;
            IEnumerable<NotificationModel> notifications = _context.Notifications
                .Where(n => n.ForUserId == accountId && (n.DateAdded > afterDate)).ToList();

            var defaultNotifications = await GetDetailedNotificationsAsync(notifications);

            if(defaultNotifications.Count() > 0)
            {
                return new GetNewNotificationsResultModel
                {
                    Notifications = defaultNotifications,
                    UnviewedCount = await GetUnviewedCountAsync(accountId)
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

            // Game Updated
            IEnumerable<UpdateGameNotificationModel> gameUpdateNotifications = await GetGameUpdateNotificationsAsync
                (notifications.Where(n => n.Type == NotificationTypeEnum.GameUpdatedForBooker || n.Type == NotificationTypeEnum.GameUpdatedForOwner)
                .Select(n => n.Id).ToArray());

            var genericNotifications = notifications.Where(n =>
            {
                return n.Type == NotificationTypeEnum.TournamentTeamRemoved ||
                n.Type == NotificationTypeEnum.NewJoinTournamentRequest ||
                n.Type == NotificationTypeEnum.JoinTeamRequestApproved ||
                n.Type == NotificationTypeEnum.JoinTeamRequest ||
                n.Type == NotificationTypeEnum.JoinTournamentRequestApproved ||
                n.Type == NotificationTypeEnum.JoinTournamentRequestRejected ||
                n.Type == NotificationTypeEnum.NewGameForBooker ||
                n.Type == NotificationTypeEnum.NewGameForOwner ||
                n.Type == NotificationTypeEnum.AssignedAsTimekeeper;
            });
            
            resultWithDetails.AddRange(genericNotifications);

            return resultWithDetails.OrderByDescending(n=>n.DateAdded);
        }
        private async Task<IEnumerable<UpdateGameNotificationModel>> GetGameUpdateNotificationsAsync(long[] NotificationIds)
        {
            if (NotificationIds.Length == 0) return await Task.FromResult<IEnumerable<UpdateGameNotificationModel>>
                    (new List<UpdateGameNotificationModel>());

            return await _context.UpdateGameNotifications
                .Where(n => NotificationIds.Contains(n.Id))
                .Include(n => n.Game).Include(n => n.Game.Court).Include(n => n.UpdatedBy)
                .Select(n => new UpdateGameNotificationDto
                {
                    Id = n.Id,
                    DateAdded = n.DateAdded,
                    ForUserId = n.ForUserId,
                    IsViewed = n.IsViewed,
                    Type = n.Type,
                    Game = n.Game,
                    UpdatedById = n.UpdatedById,
                    UpdatedbyName = n.UpdatedBy.User.UserName,
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