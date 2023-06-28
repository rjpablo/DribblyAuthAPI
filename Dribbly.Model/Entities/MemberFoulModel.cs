using Dribbly.Model.Account;
using Dribbly.Model.Fouls;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    [Table("MemberFouls")]
    public class MemberFoulModel : GameEventModel
    {
        public int FoulId { get; set; }
        public bool IsOffensive { get; set; }
        public bool IsTechnical { get; set; }
        public bool IsFlagrant { get; set; }

        #region Navigational properties
        public FoulModel Foul { get; set; }
        #endregion

        public MemberFoulModel()
        {
            Type = Enums.GameEventTypeEnum.FoulCommitted;
        }
    }
}
