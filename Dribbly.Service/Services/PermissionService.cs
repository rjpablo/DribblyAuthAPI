using Dribbly.Authentication.Models;
using Dribbly.Model;
using Dribbly.Service.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public class PermissionsService : IPermissionsService
    {
        IAuthContext _context;
        IPermissionsRepository _permissionsRepo;

        public PermissionsService(IAuthContext context,
            IPermissionsRepository permissionsRepo)
        {
            _context = context;
            _permissionsRepo = permissionsRepo;
        }

        public async Task<IEnumerable<string>> GetUserPermissionNamesAsync(string userId)
        {
            return (await _permissionsRepo.GetUserPermissionsAsync(userId))
                .OrderBy(p=>p.Name)
                .Select(p=>p.Name);
        }
    }
}