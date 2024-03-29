﻿using Dribbly.Core.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities.Groups
{
    [Table("JoinGroupRequests")]
    public class JoinGroupRequest : BaseEntityModel
    {
        [ForeignKey(nameof(Requestor))]
        public long RequestorId { get; set; }
        [ForeignKey(nameof(Group))]
        public long GroupId { get; set; }
        public AccountModel Requestor { get; set; }
        [JsonIgnore]
        public GroupModel Group { get; set; }
    }
}
