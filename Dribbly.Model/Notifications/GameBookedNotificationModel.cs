using Dribbly.Authentication.Models.Auth;
using Dribbly.Model.Games;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Notifications
{
    [Table("GameBookedNotifications")]
    public class GameBookedNotificationModel : NotificationModel
    {
        [ForeignKey(nameof(Game))]
        public long GameId { get; set; }
        /// <summary>
        /// The Id of the user who made the booking. <see cref="ApplicationIdentity.Id"/>
        /// </summary>
        [ForeignKey(nameof(BookedBy))]
        public string BookedById { get; set; }
        /// <summary>
        /// The name of the user who made the booking
        /// </summary>
        [NotMapped]
        public string BookedByName { get; set; }
        [NotMapped]
        public string CourtName { get; set; }

        public GameModel Game { get; set; }
        public ApplicationUser BookedBy { get; set; }
    }

    [NotMapped]
    public class GameBookedNotificationViewModel : NotificationModel
    {
        public long GameId { get; set; }
        public string BookedById { get; set; }
        public string BookedByName { get; set; }
        public string CourtName { get; set; }
    }
}
