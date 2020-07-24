namespace Dribbly.Model.Shared
{
    public class PhoneVerificationResultModel
    {
        public bool Successful { get; set; }

        public bool CodeWasIncorrect { get; set; }

        public long GeneratedContactId { get; set; }
    }
}
