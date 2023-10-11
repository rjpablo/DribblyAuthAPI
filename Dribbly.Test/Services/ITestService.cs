using Dribbly.Test.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dribbly.Test.Services
{
    public interface ITestService
    {
        Task<IEnumerable<TestUserActivityViewModel>> GetUserActivities();
    }
}
