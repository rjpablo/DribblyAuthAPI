using Dribbly.Service.Enums;

namespace Dribbly.Model.Shared
{
    public class GetTypeAheadSuggestionsInputModel
    {
        public string Keyword { get; set; }
        public EntityTypeEnum[] EntityTypes { get; set; }
        /// <summary>
        /// The maximun no. of results to return
        /// </summary>
        public int MaxCount { get; set; }
    }
}
