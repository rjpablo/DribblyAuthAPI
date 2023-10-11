using Dribbly.Model.Shared;
using Dribbly.Service.Services;
using Dribbly.SMS.Models;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Contacts")]
    public class ContactsController : BaseController
    {
        private IContactsService _service = null;

        public ContactsController(IContactsService service) : base()
        {
            _service = service;
        }

        [HttpPost]
        [Route("SendVerificationCode")]
        public async Task SendVerificationCodeAsync(ContactVerificationModel input)
        {
            await _service.SendVerificationCodeAsync(input);
        }

        [HttpPost]
        [Route("VerifyMobileNumber")]
        public async Task<PhoneVerificationResultModel> VerifyMobileNumber(ContactVerificationModel contact)
        {
            return await _service.VerifyMobileNumberAsync(contact);
        }
    }
}
