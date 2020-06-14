using Dribbly.Service.Models;
using Dribbly.Service.Models.Courts;
using System.Collections.Generic;
using System.Data.Entity;

namespace Dribbly.Service.Repositories
{
    public class SettingsRepository : BaseRepository<SettingModel>
    {
        AuthContext _context;

        public SettingsRepository(IAuthContext context) :base(context.Settings)
        {
            _context = new AuthContext();
        }

        public IEnumerable<SettingModel> GetAll()
        {
            return _context.Settings;
        }

    }
}