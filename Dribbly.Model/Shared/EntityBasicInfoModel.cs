﻿using Dribbly.Core.Enums;
using Dribbly.Core.Models;
using Dribbly.Model.Account;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Shared
{
    [NotMapped]
    public class EntityBasicInfoModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public EntityStatusEnum EntityStatus { get; set; }
        public virtual MultimediaModel Photo { get; set; }
        public EntityTypeEnum Type { get; set; }
        public string IconUrl { get; set; }

        public EntityBasicInfoModel()
        {

        }

        public EntityBasicInfoModel(PlayerModel account)
        {
            Id = account.Id;
            Name = account.User != null ? account.User.UserName : account.Username;
            Photo = account.ProfilePhoto;
            IconUrl = account.IconUrl;
            Type = EntityTypeEnum.Account;
            EntityStatus = account.EntityStatus;
        }
    }
}