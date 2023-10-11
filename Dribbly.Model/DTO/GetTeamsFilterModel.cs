using Dribbly.Core.Models;
using Dribbly.Model.Enums;
using System.Collections.Generic;

namespace Dribbly.Model.DTO
{
    public class GetTeamsFilterModel : PagedGetInputModel
    {
        public StatEnum SortBy { get; set; } = StatEnum.OverallScore;
        public SortDirectionEnum SortDirection { get; set; } = SortDirectionEnum.Ascending;
        public List<long> CourtIds { get; set; } = new List<long>();

    }
}
