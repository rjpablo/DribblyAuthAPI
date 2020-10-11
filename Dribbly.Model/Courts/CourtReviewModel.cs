using Dribbly.Authentication.Models.Auth;
using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Courts
{
    [Table("CourtReviews")]
    public class CourtReviewModel : ReviewModel
    {
        [ForeignKey("Court"), Required]
        public long CourtId { get; set; }

        // navigation properties
        public virtual CourtModel Court { get; set; }

    }
}