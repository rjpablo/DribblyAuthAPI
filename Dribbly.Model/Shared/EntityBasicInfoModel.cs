using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Service.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Shared
{
    [NotMapped]
    public class EntityBasicInfoModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public EntityStatusEnum EntityStatus { get; set; }
        public virtual PhotoModel Photo { get; set; }
        public EntityTypeEnum Type { get; set; }
        public string IconUrl { get; set; }

        public EntityBasicInfoModel()
        {

        }

        public EntityBasicInfoModel(AccountModel account)
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