using Dribbly.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Notifications
{
    [Table("Notifications")]
    public class NotificationModel : BaseEntityModel, INotificationModel
    {
        /// <summary>
        /// The ID of the user that this notification is intended for
        /// </summary>
        public long ForUserId { get; set; }
        public NotificationTypeEnum Type { get; set; }
        public bool IsViewed { get; set; }
        public string AdditionalInfo { get; set; }

        public NotificationModel() { }

        public NotificationModel(NotificationTypeEnum type)
        {
            Type = type;
            DateAdded = DateTime.UtcNow;
        }
    }

    public interface INotificationModel : IBaseEntityModel
    {
        bool IsViewed { get; set; }
        long ForUserId { get; set; }
        NotificationTypeEnum Type { get; set; }
        string AdditionalInfo { get; set; }
    }
}