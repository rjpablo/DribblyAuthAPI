using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Account
{
    [NotMapped]
    public class AccountBasicInfoModel: IIndexedEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public EntityStatusEnum EntityStatus { get; set; }
        public long IdentityUserId { get; set; }
        public GenderEnum? Gender { get; set; }
        public bool IsPublic { get; set; }
        public EntityTypeEnum EntityType { get; set; }
        public string IconUrl { get; set; }
        public DateTime DateAdded { get; set; }
        public string Description { get; set; }

        public AccountBasicInfoModel()
        {

        }

        public AccountBasicInfoModel(AccountModel account)
        {
            IdentityUserId = account.IdentityUserId;
            Id = account.Id;
            Username = account.User != null ? account.User.UserName : account.Username;
            Name = account.Name;
            Gender = account.Gender;
            IconUrl = account.IconUrl;
            EntityStatus = account.EntityStatus;
            IsPublic = account.IsPublic;
            EntityType = account.EntityType;
            DateAdded = account.DateAdded;
            Description = account.Description;
        }
    }
}