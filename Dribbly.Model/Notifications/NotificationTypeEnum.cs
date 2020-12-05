using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.Notifications
{
    public enum NotificationTypeEnum
    {
        // Notification for court owner when a new booking is made by booker
        NewBookingForOwner,
        // Notification for booker when a new booking is added by court owner
        NewBookingForBooker,
        /// <summary>
        /// Notificaion for team owner/manage when a player creates a request to join the team
        /// </summary>
        JoinTeamRequest,
        /// <summary>
        /// Notification for player when his/her request to join a team gets approved.
        /// </summary>
        JoinTeamRequestApproved,
    }
}
