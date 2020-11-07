using Dribbly.Core.Exceptions;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Shared;
using Dribbly.SMS;
using Dribbly.SMS.Models;
using Dribbly.SMS.Services;
using System;
using System.IdentityModel;

namespace Dribbly.Service.Services
{
    public class ContactsService: BaseEntityService<ContactModel>, IContactsService
    {
        IAuthContext _context;
        ISecurityUtility _securityUtility;
        ISmsService _smsService;

        public ContactsService(IAuthContext context,
            ISecurityUtility securityUtility) : base(context.Contacts)
        {
            _context = context;
            _securityUtility = securityUtility;
            _smsService = new SMSService();
        }

        public void SendVerificationCode(ContactVerificationModel input)
        {
            _smsService.SendVerificationCode(input.ContactNumber);
        }

        public PhoneVerificationResultModel VerifyMobileNumber(ContactVerificationModel input)
        {
            PhoneVerificationResultModel result = new PhoneVerificationResultModel();
            string status = _smsService.VerifyMobileNumber(input.ContactNumber, input.Code);

            if (status == "approved") //status can either be 'approved' or 'pending'
            {
                ContactModel contact = new ContactModel
                {
                    Number = input.ContactNumber,
                    AddedBy = _securityUtility.GetUserId()
                };

                Add(contact);
                _context.SaveChanges();
                result.Successful = true;
                result.GeneratedContactId = contact.Id;
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
