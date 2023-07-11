using Dribbly.Service.Enums;
using System.Collections.Generic;

namespace Dribbly.Model.Shared
{
    public class GetTypeAheadSuggestionsInputModel
    {
        public string Keyword { get; set; }
        public List<EntityTypeEnum> EntityTypes { get; set; } = new List<EntityTypeEnum>();
        /// <summary>
        /// The maximun no. of results to return
        /// </summary>
        public int? MaxCount { get; set; }
    }
}
