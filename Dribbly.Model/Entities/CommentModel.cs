using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    [Table("Comments")]
    public class CommentModel : BaseEntityModel
    {
        public string Message { get; set; }
        [ForeignKey(nameof(AddedBy))]
        public long AddedById { get; set; }
        public CommentedOnTypeEnum CommentedOnType { get; set; }
        public long CommentedOnId { get; set; }
        public PlayerModel AddedBy { get; set; }
    }
}
