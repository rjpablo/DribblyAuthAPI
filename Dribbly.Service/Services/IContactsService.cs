using Dribbly.Model.Shared;
using Dribbly.SMS.Models;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public interface IContactsService
    {
        Task SendVerificationCodeAsync(ContactVerificationModel input);

        Task<PhoneVerificationResultModel> VerifyMobileNumberAsync(ContactVerificationModel contact);
    }
}