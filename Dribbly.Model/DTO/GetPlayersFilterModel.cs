using Dribbly.Core.Models;
using Dribbly.Model.Enums;
using Dribbly.Service.Enums;
using System;
using System.Collections.Generic;

namespace Dribbly.Model.DTO
{
    public class GetPlayersFilterModel : PagedGetInputModel
    {
        public GetPlayersSortByEnum SortBy { get; set; } = GetPlayersSortByEnum.OverallScore;
        public SortDirectionEnum SortDirection { get; set; } = SortDirectionEnum.Ascending;
        public List<long> CourtIds { get; set; } = new List<long>();
        public DateTime? JoinBeforeDate { get; set; }
        public PlayerPositionEnum? Position { get; set; }
        public string PlaceId { get; set; }
    }
}
