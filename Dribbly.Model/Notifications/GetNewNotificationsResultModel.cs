using System.Collections.Generic;

namespace Dribbly.Model.Notifications
{
    public class GetNewNotificationsResultModel
    {
        public UnviewedCountModel UnviewedCount { get; set; }
        public IEnumerable<object> Notifications { get; set; }
    }
}
