using Dribbly.Model.Account;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Courts
{
    [NotMapped]
    public class CourtDetailsViewModel : CourtModel
    {
        public long FollowerCount { get; set; }

        public virtual AccountBasicInfoModel Owner { get; set; }

        public bool IsFollowed { get; set; }

        public CourtDetailsViewModel(CourtModel c)
        {
            Id = c.Id;
            DateAdded = c.DateAdded;
            OwnerId = c.OwnerId;
            Name = c.Name;
            RatePerHour = c.RatePerHour;
            Rating = c.Rating;
            PrimaryPhotoId = c.PrimaryPhotoId;
            Latitude = c.Latitude;
            Longitude = c.Longitude;
            IsFreeToPlay = c.IsFreeToPlay;
            ContactId = c.ContactId;
            AdditionalInfo = c.AdditionalInfo;
            Photos = c.Photos;
            Videos = c.Videos;
            Contact = c.Contact;
            PrimaryPhoto = c.PrimaryPhoto;
        }
    }
}
