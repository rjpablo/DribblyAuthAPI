using Dribbly.Test.Services;
using Unity;

namespace Dribbly.Test
{
    public class TestsConfig
    {
        public static void RegisterComponents(IUnityContainer container)
        {
            // Register Core components

            container.RegisterType<ITestService, TestService>();
        }
    }
}
