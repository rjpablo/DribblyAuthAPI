using Dribbly.Model.Enums;

namespace Dribbly.Model.GameEvents
{
    public class RecordTimeoutResultModel
    {
        public TimeoutTypeEnum Type { get; set; }
        public int TimeoutsLeft { get; set; }
        public int FullTimeoutsUsed { get; set; }
        public int ShortTimeoutsUsed { get; set; }
        public long? TeamId { get; set; }
    }
}
