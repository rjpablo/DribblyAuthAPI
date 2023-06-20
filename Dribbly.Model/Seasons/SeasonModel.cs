using Dribbly.Core.Models;
using Dribbly.Model.Enums;
using Dribbly.Model.Leagues;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Seasons
{
    [Table("Seasons")]
    public class SeasonModel:BaseEntityModel
    {
        public long AddedById { get; set; }
        public string Name { get; set; }
        [ForeignKey(nameof(League))]
        public long LeagueId { get; set; }
        public LeagueModel League { get; set; }
        public SeasonStatusEnum Status { get; set; }
    }
}
