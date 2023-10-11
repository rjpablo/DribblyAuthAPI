using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.Enums
{
    public enum GameEventTypeEnum
    {
        ShotMade = 1,
        ShotMissed = 2,
        FoulCommitted = 3,
        ShotBlock = 4,
        Assist = 5,
        OffensiveRebound = 6,
        DefensiveRebound = 7,
        Jumpball = 8,
        Timeout = 9,
        ChangeLineup = 10,
        TurnOver = 11,
        Steal = 12,
        FreeThrowMade = 13,
        FreeThrowMissed = 14
    }
}
