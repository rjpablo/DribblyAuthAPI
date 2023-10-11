namespace Dribbly.SMS.Services
{
    public interface ISmsService
    {
        void SendVerificationCode(string mobileNumber);

        string VerifyMobileNumber(string mobileNumber, string code);
    }
}
