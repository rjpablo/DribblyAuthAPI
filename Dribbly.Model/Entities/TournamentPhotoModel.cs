using Dribbly.Core.Models;
using Dribbly.Model.Courts;
using Dribbly.Model.Tournaments;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    [Table("TournamentPhotos")]
    public class TournamentPhotoModel : BaseEntityModel
    {
        [ForeignKey(nameof(Tournament))]
        public long TournamentId { get; set; }
        [ForeignKey(nameof(Photo))]
        public long PhotoId { get; set; }
        public TournamentModel Tournament { get; set; }
        public MultimediaModel Photo { get; set; }
    }
}
