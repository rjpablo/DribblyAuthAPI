using Dribbly.Service.Models.Account;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public interface IAccountsService
    {
        Task<AccountModel> GetAccountByUsername(string userName);

        Task AddAsync(AccountModel account);

        Task<AccountSettingsModel> GetAccountSettingsAsync(string userId);
    }
}