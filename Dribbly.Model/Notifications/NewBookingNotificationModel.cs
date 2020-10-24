using Dribbly.Authentication.Models.Auth;
using Dribbly.Model.Bookings;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Notifications
{
    [Table("NewBookingNotifications")]
    public class NewBookingNotificationModel : NotificationModel
    {
        [ForeignKey(nameof(Booking))]
        public long BookingId { get; set; }
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

        public BookingModel Booking { get; set; }
        public ApplicationUser BookedBy { get; set; }
    }

    [NotMapped]
    public class NewBookingNotificationViewModel : NotificationModel
    {
        public long BookingId { get; set; }
        public string BookedById { get; set; }
        public string BookedByName { get; set; }
        public string CourtName { get; set; }
    }
}
