using Dribbly.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities.Groups
{
    [Table("GroupMembers")]
    public class GroupMemberModel
    {
        [Column(Order = 1), Key]
        [ForeignKey(nameof(Account))]
        public long AccountId { get; set; }
        [ForeignKey(nameof(Group))]
        [Column(Order = 2), Key]
        public long GroupId { get; set; }
        public AccountModel Account { get; set; }
        public GroupModel Group { get; set; }
        public DateTime DateJoined { get; set; }
        public string Name { get => Account.Name; }
        public MultimediaModel ProfilePhoto { get => Account.ProfilePhoto; }
        public string IconUrl { get => Account.IconUrl; }
    }
}
