using Dribbly.Core.Utilities;
using System;
using System.Security.Principal;
using System.Web;
using Unity;

namespace Dribbly.Core
{
    public class CoreUnityConfig
    {
        public static void RegisterComponents(IUnityContainer container)
        {
            // register all your components with the container here
            // it is NOT necessary to register your controllers

            container.RegisterFactory<HttpContextBase>(_ => new HttpContextWrapper(HttpContext.Current));

            // Register Core components

            container.RegisterType<ISecurityUtility, SecurityUtility>();
        }
    }
}
