using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.DTO.Account
{
    public class UpdateEmailInput
    {
        [Email]
        public string NewEmail { get; set; }
    }
}
