using DribblyAuthAPI.Models;
using DribblyAuthAPI.Models.Account;
using DribblyAuthAPI.Models.Auth;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DribblyAuthAPI.Repositories
{
    public class AccountRepository: BaseRepository<AccountModel>, IAccountRepository
    {
        IAuthContext _context;
        IAuthRepository _authRepo;

        public AccountRepository(IAuthContext context, IAuthRepository authRepo) :base(context.Accounts)
        {
            _context = context;
            _authRepo = authRepo;
        }

        public async Task<AccountModel> GetAccountByUsername(string userName)
        {
            ApplicationUser user = await _authRepo.FindUserByName(userName);

            if(user == null)
            {
                //TODO: log IdentityUser not found
                return null;
            }

            AccountModel account = await _context.Accounts.SingleOrDefaultAsync(a => a.IdentityUserId == user.Id);

            if(account == null)
            {
                //TODO: log identityUser exists but Account doesn't
                return null;
            }

            account.Merge(user);
            return account;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AccountsRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}