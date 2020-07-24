using Dribbly.Model.Shared;
using Dribbly.SMS.Models;

namespace Dribbly.Service.Services
{
    public interface IContactsService
    {
        void SendVerificationCode(ContactVerificationModel contact);

        PhoneVerificationResultModel VerifyMobileNumber(ContactVerificationModel contact);
    }
}