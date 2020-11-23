using Dribbly.Model.Account;
using Dribbly.Service.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Shared
{
    public interface IIndexedEntity
    {
        long Id { get; set; }
        EntityTypeEnum EntityType { get; }
        string Name { get; }
        string IconUrl { get; }
        EntityStatusEnum Status { get; set; }
        string Description { get; }
        DateTime DateAdded { get; set; }
    }

    [Table("IndexedEntities")]
    public class IndexedEntityModel: IIndexedEntity
    {
        [Key, Column(Order = 1)]
        public long Id { get; set; }

        [Key, Column("Type",Order = 2)]
        public EntityTypeEnum EntityType { get; set; }
        public string Name { get; set; }

        public string IconUrl { get; set; }

        public EntityStatusEnum Status { get; set; }

        public string Description { get; set; }

        public DateTime DateAdded { get; set; }

        public IndexedEntityModel() { }

        public IndexedEntityModel(AccountModel account)
        {
            Id = account.Id;
            Name = account.Username;
            EntityType = EntityTypeEnum.Account;
            DateAdded = account.DateAdded;
            Status = EntityStatusEnum.Active;
            IconUrl = account.ProfilePhoto?.Url;
        }

        public IndexedEntityModel(IIndexedEntity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            EntityType = entity.EntityType;
            Description = entity.Description;
            DateAdded = entity.DateAdded;
            Status = EntityStatusEnum.Active;
            IconUrl = entity.IconUrl;
        }

        public IndexedEntityModel(string text, long id, string description, string iconUrl, EntityTypeEnum type)
        {
            Name = text;
            Id = id;
            Description = description;
            IconUrl = iconUrl;
            EntityType = type;
        }

        public ChoiceItemModel<long> ToChoiceItemModel()
        {
            return new ChoiceItemModel<long>
            {
                Text = Name,
                Value = Id,
                IconUrl = IconUrl
            };
        }
    }
}
