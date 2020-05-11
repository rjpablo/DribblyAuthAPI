using DribblyAuthAPI.Models;
using DribblyAuthAPI.Models.Account;
using DribblyAuthAPI.Repositories;
using System.Threading.Tasks;

namespace DribblyAuthAPI.Services
{
    public class AccountsService : BaseEntityService<AccountModel>, IAccountsService
    {
        IAccountRepository _accountRepo;
        IAuthContext _context;

        public AccountsService(IAuthContext context,
            IAccountRepository accountRepo) : base(context.Accounts)
        {
            _accountRepo = accountRepo;
            _context = context;
        }

        public Task<AccountModel> GetAccountByUsername(string userName)
        {
            return _accountRepo.GetAccountByUsername(userName);
        }

        public async Task AddAsync(AccountModel account)
        {
            Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task<AccountSettingsModel> GetAccountSettingsAsync(string userId)
        {
            return await Task.FromResult(new AccountSettingsModel());
        }

    }
}