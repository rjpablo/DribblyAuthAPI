using Dribbly.Model.Shared;
using Dribbly.Service.Services;
using Dribbly.SMS.Models;
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
        public void SendVerificationCode(ContactVerificationModel contact)
        {
            _service.SendVerificationCode(contact);
        }

        [HttpPost]
        [Route("VerifyMobileNumber")]
        public PhoneVerificationResultModel VerifyMobileNumber(ContactVerificationModel contact)
        {
            return _service.VerifyMobileNumber(contact);
        }
    }
}
