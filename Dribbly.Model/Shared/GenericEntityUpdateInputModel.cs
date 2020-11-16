using System.Collections.Generic;

namespace Dribbly.Model.Shared
{
    public class GenericEntityUpdateInputModel
    {
        public long Id { get; set; }

        public Dictionary<string, object> Properties { get; set; }
    }
}
