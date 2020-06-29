using Dribbly.Authentication.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public interface IPermissionsService
    {
        Task<IEnumerable<string>> GetUserPermissionNamesAsync(string userId);
    }
}