using Dribbly.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.Teams
{
    public class TeamMembershipViewModel
    {
        public long TeamId { get; set; }
        public long MemberAccountId { get; set; }
        public PlayerPositionEnum Position { get; set; }
        public bool IsCurrentMember { get; set; }
        public bool IsFormerMember { get; set; }
        public bool IsCurrentCoach { get; set; }
        public bool HasPendingJoinRequest { get; set; }
    }
}
