using DribblyAuthAPI.Models;
using DribblyAuthAPI.Models.Courts;
using System.Collections.Generic;
using System.Data.Entity;

namespace DribblyAuthAPI.Repositories
{
    public class CourtsRepository: BaseRepository<CourtModel>
    {
        AuthContext _context;

        public CourtsRepository(IAuthContext context) :base(context.Courts)
        {
            _context = new AuthContext();
        }

        public IEnumerable<CourtModel> GetAll()
        {
            return _context.Courts;
        }

    }
}