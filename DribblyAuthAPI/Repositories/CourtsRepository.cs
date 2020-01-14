using DribblyAuthAPI.Models;
using DribblyAuthAPI.Models.Courts;
using System.Collections.Generic;

namespace DribblyAuthAPI.Repositories
{
    public class CourtsRepository
    {
        AuthContext _context;

        public CourtsRepository()
        {
            _context = new AuthContext();
        }

        public IEnumerable<CourtModel> GetAll()
        {
            return _context.Courts;
        }

    }
}