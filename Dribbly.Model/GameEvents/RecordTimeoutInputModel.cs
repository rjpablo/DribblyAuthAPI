using Dribbly.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.GameEvents
{
    public class RecordTimeoutInputModel
    {
        public long GameId { get; set; }
        /// <summary>
        /// The ID of the team that called the timeout
        /// </summary>
        public long? TeamId { get; set; }
        public TimeoutTypeEnum Type { get; set; }
        public int Period { get; set; }
        public int ClockTime { get; set; }
    }
}
