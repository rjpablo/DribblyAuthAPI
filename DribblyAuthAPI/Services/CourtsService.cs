using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DribblyAuthAPI.Models;
using DribblyAuthAPI.Models.Courts;

namespace DribblyAuthAPI.Services
{
    public class CourtsService : BaseService<CourtModel>, ICourtsService
    {
        IAuthContext _context;
        public CourtsService(IAuthContext context):base(context.Courts)
        {
            _context = context;
        }

        public IEnumerable<CourtModel> GetAll()
        {
            return All();
        }

        public CourtModel GetCourt(long id)
        {
            return GetById(id);
        }

        public void Register(CourtModel court)
        {
            Add(court);
            _context.SaveChanges();
        }

        public void UpdateCourt(CourtModel court)
        {
            Update(court);
            _context.SaveChanges();
        }
    }
}