using Dribbly.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Notifications
{
    [Table("Notifications")]
    public abstract class NotificationModel: BaseEntityModel
    {
        /// <summary>
        /// The ID of the user that this notification is intended for
        /// </summary>
        public string ForUserId { get; set; }
        public NotificationTypeEnum Type { get; set; }
        public bool IsViewed { get; set; }
    }

    public interface INotificationModel
    {
        bool IsViewed { get; set; }
        string ForUserId { get; set; }
        NotificationTypeEnum Type { get; set; }
    }
}