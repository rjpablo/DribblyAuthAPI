using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.Notifications
{
    public enum NotificationTypeEnum
    {
        #region Game-Related 0-19

        // Notification for court owner when a new booking is made by booker
        NewGameForOwner = 0,
        // Notification for booker when a new booking is added by court owner
        NewGameForBooker = 1,
        /// <summary>
        /// Notification for booker when the game he booked is updated
        /// </summary>
        GameUpdatedForBooker = 2,
        /// <summary>
        /// Notification for court owner when a booker updates a game
        /// </summary>
        GameUpdatedForOwner = 3,
        /// <summary>
        /// Notification for the assigned time keeper of a game
        /// </summary>
        AssignedAsTimekeeper = 4,

        #endregion

        #region Team-Related 20-39

        /// <summary>
        /// Notificaion for team owner/manage when a player creates a request to join the team
        /// </summary>
        JoinTeamRequest = 20,
        /// <summary>
        /// Notification for player when his/her request to join a team gets approved.
        /// </summary>
        JoinTeamRequestApproved = 21,

        #endregion

        #region Booking-Related 40-59
        /// <summary>
        /// Sent to court owner when a user books his/her court
        /// </summary>
        BookingNotificationForCourtOwner = 40,
        /// <summary>
        /// For when a court owner makes a booking for another user. This is sent to the user that the booking was made for.
        /// </summary>
        BookingNotificationForBooker = 41,
        #endregion

        #region Tournament-Related 60-79
        NewJoinTournamentRequest = 60,
        JoinTournamentRequestApproved = 61,
        JoinTournamentRequestRejected = 62,
        TournamentTeamRemoved = 63,
        #endregion
    }
}
