using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dribbly.Authentication.Models.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public string Salt { get; set; }
    }
}