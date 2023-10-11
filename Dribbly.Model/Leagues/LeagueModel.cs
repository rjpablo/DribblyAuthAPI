using Dribbly.Core.Enums;
using Dribbly.Core.Models;
using Dribbly.Model.Seasons;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Leagues
{
    [Table("Leagues")]
    public class LeagueModel : BaseEntityModel, IIndexedEntity
    {
        public EntityTypeEnum EntityType => EntityTypeEnum.League;

        public string Name { get; set; }

        public string IconUrl { get; set; }

        public EntityStatusEnum EntityStatus { get; set; }

        public string Description { get; set; }

        public long AddedById { get; set; }
        public ICollection<SeasonModel> Seasons { get; set; }
    }
}
