using Dribbly.Core.Enums;
using Dribbly.Model.Seasons;
using System;
using System.Collections.Generic;

namespace Dribbly.Model.Leagues
{
    public class LeagueViewerModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public string IconUrl { get; set; }

        public EntityStatusEnum EntityStatus { get; set; }

        public string Description { get; set; }

        public long AddedById { get; set; }
        public DateTime DateAdded { get; set; }

        public IEnumerable<SeasonModel> Seasons { get; set; }


        public LeagueViewerModel(LeagueModel entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            IconUrl = entity.IconUrl;
            EntityStatus = entity.EntityStatus;
            Description = entity.Description;
            AddedById = entity.AddedById;
            DateAdded = entity.DateAdded;
        }
    }
}
