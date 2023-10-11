using Dribbly.Email.Models;
using System.Threading.Tasks;

namespace Dribbly.Email.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage message);
    }
}
