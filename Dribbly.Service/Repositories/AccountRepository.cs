using Dribbly.Authentication.Models.Auth;
using Dribbly.Model;
using Dribbly.Model.Account;
using Dribbly.Service.Enums;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
{
    public class AccountRepository : BaseRepository<AccountModel>, IAccountRepository
    {
        IAuthContext _context;
        IAuthRepository _authRepo;

        public AccountRepository(IAuthContext context, IAuthRepository authRepo) : base(context.Accounts)
        {
            _context = context;
            _authRepo = authRepo;
        }

        public async Task<AccountModel> GetAccountByUsername(string userName)
        {
            AccountModel account = await _context.Accounts.Include(a => a.ProfilePhoto).Include(a => a.User)
                .Include(a => a.HomeCourt).Include(a => a.HomeCourt.PrimaryPhoto).SingleOrDefaultAsync
                (a => a.User.UserName.Equals(userName, System.StringComparison.OrdinalIgnoreCase));
            return account;
        }

        /// <summary>
        /// Get account via identityUserId
        /// </summary>
        /// <param name="identityUserId"></param>
        /// <returns></returns>
        public async Task<AccountModel> GetAccountByIdentityId(long identityUserId)
        {
            return await _context.Accounts.Include(a => a.ProfilePhoto).Include(a => a.User)
                .SingleOrDefaultAsync(a => a.IdentityUserId == identityUserId);
        }

        /// <summary>
        /// Get account via Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<AccountModel> GetAccountById(long Id)
        {
            return await _context.Accounts.Include(a => a.ProfilePhoto).Include(a => a.User).Include(a => a.HomeCourt)
                .SingleOrDefaultAsync(a => a.Id == Id);
        }

        public async Task<long?> GetIdentityUserAccountId(string identityUserId)
        {
            return (await _context.Accounts.SingleOrDefaultAsync(a => a.IdentityUserId.ToString() == identityUserId))?.Id;
        }

        public async Task<AccountBasicInfoModel> GetAccountBasicInfo(long userId)
        {
            AccountModel account = await _dbSet.Include(a => a.ProfilePhoto).Include(a => a.User)
                .SingleOrDefaultAsync(a => a.IdentityUserId == userId);
            if (account == null)
            {
                return null;
            }
            return account.ToBasicInfo();
        }

        public void ActivateByUserId(string userid)
        {
            AccountModel account = _dbSet.SingleOrDefault(a => a.IdentityUserId.ToString() == userid);
            if (account != null && account.Status != EntityStatusEnum.Active)
            {
                account.Status = EntityStatusEnum.Active;
            }
        }

        public IQueryable<AccountModel> SearchAccounts(AccountSearchInputModel input)
        {
            return _dbSet.Include(a => a.ProfilePhoto).Include(a => a.User)
                .Where(a => a.User.UserName.Contains(input.NameSegment) && !input.ExcludeIds.Contains(a.IdentityUserId));
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