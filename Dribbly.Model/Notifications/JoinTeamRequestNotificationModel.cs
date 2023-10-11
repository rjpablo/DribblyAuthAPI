using Dribbly.Authentication.Models.Auth;
using Dribbly.Identity.Models;
using Dribbly.Model.Bookings;
using Dribbly.Model.Teams;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Notifications
{
    [Table("JoinTeamRequestNotifications")]
    public class JoinTeamRequestNotificationModel : NotificationModel
    {
        [ForeignKey(nameof(Request))]
        public long RequestId { get; set; }

        public JoinTeamRequestModel Request { get; set; }
    }

    [NotMapped]
    public class JoinTeamRequestNotificationDto : JoinTeamRequestNotificationModel
    {
        public JoinTeamRequestNotificationDto(JoinTeamRequestNotificationModel n)
        {
            Id = n.Id;
            DateAdded = n.DateAdded;
            ForUserId = n.ForUserId;
            IsViewed = n.IsViewed;
            Type = n.Type;
            
        }
    }

    [NotMapped]
    public class JoinTeamRequestNotificationViewModel : NotificationModel
    {
        public long RequestorId { get; set; }
        public string RequestorName { get; set; }
        public string TeamName { get; set; }
        public long TeamId { get; set; }
    }
}
