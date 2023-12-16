using Dribbly.Core.Enums;
using Dribbly.Core.Models;
using Dribbly.Model.Account;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    [Table("Comments")]
    public class CommentModel : BaseEntityModel
    {
        public string Message { get; set; }
        [ForeignKey(nameof(AddedBy))]
        public long AddedById { get; set; }
        public EntityTypeEnum CommentedOnType { get; set; }
        public long CommentedOnId { get; set; }
        public PlayerModel AddedBy { get; set; }
    }
}
