using Dribbly.Core.Models;
using Dribbly.Model.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    public abstract class UserReactionModel : BaseEntityModel
    {
        public ReactionTypeEnum Type { get; set; }
        [ForeignKey(nameof(Reactor))]
        public long ReactorId { get; set; }
        public AccountModel Reactor { get; set; }
    }
}
