using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dribbly.Identity.Models
{
    public class ApplicationUser : IdentityUser<long, CustomUserLogin, CustomUserRole,
    CustomUserClaim>
    {
        public string Salt { get; set; }
    }
}