using System;
using Twilio;
using Twilio.Rest.Verify.V2;
using Twilio.Rest.Verify.V2.Service;

namespace Dribbly.SMS.Services
{
    public class SMSService : ISmsService
    {
        const string accountSid = "AC75ad9f6d6ef517b3fd903a2b68ccaae4";
        const string authToken = "fb7f7a019f94377809c7e6011cd4836b";

        public static void Init()
        {
            TwilioClient.Init(accountSid, authToken);
        }

        public void SendVerificationCode(string mobileNumber)
        {
            try
            {
                var verification = VerificationResource.Create(
                    to: mobileNumber,
                    channel: "sms",
                    pathServiceSid: "VAa844a4579647ada4b02b1a25e1eb2a8b"
                );
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string VerifyMobileNumber(string mobileNumber, string code)
        {
            var verificationCheck = VerificationCheckResource.Create(
                to: mobileNumber,
                code: code,
                pathServiceSid: "VAa844a4579647ada4b02b1a25e1eb2a8b"
            );

            return verificationCheck.Status;
        }
    }
}
