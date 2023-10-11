using Dribbly.Model;
using Dribbly.Test.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Test.Services
{
    public class TestService: ITestService
    {
        private readonly IAuthContext _context;

        public TestService(IAuthContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TestUserActivityViewModel>> GetUserActivities()
        {
            var activities = await _context.UserActivities.OrderByDescending(a => a.DateAdded).ToListAsync();
            return activities.Select(a => a.ToTestViewModel());
        }
    }
}
