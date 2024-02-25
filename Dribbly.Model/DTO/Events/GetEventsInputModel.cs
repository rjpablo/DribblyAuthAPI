using Dribbly.Core.Models;

namespace Dribbly.Model.DTO.Events
{
    public class GetEventsInputModel: PagedGetInputModel
    {
        public bool UpcomingOnly { get; set; } = true;
    }
}

