using Dribbly.Authentication.Models.Auth;
using Dribbly.Identity.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Games;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Notifications
{
    [Table("UpdateGameNotifications")]
    public class UpdateGameNotificationModel : NotificationModel
    {
        [ForeignKey(nameof(Game))]
        public long GameId { get; set; }
        /// <summary>
        /// The Id of the user who made the Game. <see cref="ApplicationIdentity.Id"/>
        /// </summary>
        [ForeignKey(nameof(UpdatedBy))]
        public long UpdatedById { get; set; }
        /// <summary>
        /// The name of the user who made the Game
        /// </summary>
        [NotMapped]
        public string UpdatedbyName { get; set; }
        [NotMapped]
        public string CourtName { get; set; }

        public GameModel Game { get; set; }
        public PlayerModel UpdatedBy { get; set; }
    }

    [NotMapped]
    public class UpdateGameNotificationDto : UpdateGameNotificationModel
    {
        
    }

    [NotMapped]
    public class UpdateGameNotificationViewModel : NotificationModel
    {
        public long GameId { get; set; }
        public string BookedById { get; set; }
        public string BookedByName { get; set; }
        public string CourtName { get; set; }
    }
}
