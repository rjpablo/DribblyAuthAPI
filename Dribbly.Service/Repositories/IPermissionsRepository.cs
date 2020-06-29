using Dribbly.Authentication.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
{
    public interface IPermissionsRepository
    {
        Task<IEnumerable<PermissionModel>> GetUserPermissionsAsync(string userId);
    }
}
