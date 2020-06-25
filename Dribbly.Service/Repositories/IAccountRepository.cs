using Dribbly.Model.Account;
using System;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
{
    public interface IAccountRepository: IDisposable
    {
        Task<AccountModel> GetAccountByUsername(string userName);

        Task<AccountBasicInfoModel> GetAccountBasicInfo(string userId);
    }
}