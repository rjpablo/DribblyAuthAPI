using Dribbly.Core.Exceptions;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using Dribbly.Service.Services.Shared;
using Dribbly.SMS;
using Dribbly.SMS.Models;
using Dribbly.SMS.Services;
using System;
using System.IdentityModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public class ContactsService: BaseEntityService<ContactModel>, IContactsService
    {
        private readonly IAuthContext _context;
        private readonly ISecurityUtility _securityUtility;
        private readonly ICommonService _commonService;
        private readonly ISmsService _smsService;

        public ContactsService(IAuthContext context,
            ISecurityUtility securityUtility,
            ICommonService commonService) : base(context.Contacts, context)
        {
            _context = context;
            _securityUtility = securityUtility;
            _commonService = commonService;
            _smsService = new SMSService();
        }

        public async Task SendVerificationCodeAsync(ContactVerificationModel input)
        {
            await _commonService.AddUserContactActivity(UserActivityTypeEnum.RequestVerificationCode, null, input.ContactNumber);
            _smsService.SendVerificationCode(input.ContactNumber);
        }

        public async Task<PhoneVerificationResultModel> VerifyMobileNumberAsync(ContactVerificationModel input)
        {
            PhoneVerificationResultModel result = new PhoneVerificationResultModel();
            string status = _smsService.VerifyMobileNumber(input.ContactNumber, input.Code);

            if (status == "approved") //status can either be 'approved' or 'pending'
            {
                ContactModel contact = new ContactModel
                {
                    Number = input.ContactNumber,
                    AddedBy = _securityUtility.GetAccountId().Value
                };

                Add(contact);
                _context.SaveChanges();
                result.Successful = true;
                result.GeneratedContactId = contact.Id;
                await _commonService.AddUserContactActivity(UserActivityTypeEnum.VerifyContact, null, input.ContactNumber);
            }
            else if (status == "pending")
            {
                result.CodeWasIncorrect = true;
            }
            else
            {
                throw new DribblyInvalidOperationException("Mobile number verification failed. Verification Status: " + status);
            }

            return result;
        }
    }
}
