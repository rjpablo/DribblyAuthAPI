using Dribbly.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    public class FeaturedEntity
    {
        [Column(Order =1), Key]
        public long EntityId { get; set; }
        [Column(Order = 2), Key]
        public EntityTypeEnum EntityType { get; set; }
        public int? Order { get; set; }
    }
}
