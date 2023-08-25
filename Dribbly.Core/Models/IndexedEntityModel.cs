using Dribbly.Core.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Core.Models
{
    /// <summary>
    /// Entities that can be searched globally and classes that are used for the entitystub component
    /// should implement this interface
    /// </summary>
    public interface IIndexedEntity
    {
        long Id { get; set; }
        EntityTypeEnum EntityType { get; }
        string Name { get; }
        string IconUrl { get; }
        EntityStatusEnum EntityStatus { get; }
        string Description { get; }
        DateTime DateAdded { get; set; }
    }

    [Table("IndexedEntities")]
    public class IndexedEntityModel : IIndexedEntity
    {
        [Key, Column(Order = 1)]
        public long Id { get; set; }

        [Key, Column("Type", Order = 2)]
        public EntityTypeEnum EntityType { get; set; }
        public string Name { get; set; }

        public string IconUrl { get; set; }

        public EntityStatusEnum EntityStatus { get; set; }

        public string Description { get; set; }

        public DateTime DateAdded { get; set; }

        // This column is not added to IIndexedEntity because
        // only IndexedEntityModel needs it
        public string AdditionalData { get; set; }

        public IndexedEntityModel() { }

        public IndexedEntityModel(AccountModel account, string username)
        {
            Id = account.Id;
            Name = account.Name;
            EntityType = EntityTypeEnum.Account;
            DateAdded = account.DateAdded;
            EntityStatus = EntityStatusEnum.Active;
            IconUrl = account.ProfilePhoto?.Url;
            AdditionalData = JsonConvert.SerializeObject(new { username = username });
        }

        public IndexedEntityModel(IIndexedEntity entity, string additionalData = null)
        {
            Id = entity.Id;
            Name = entity.Name;
            EntityType = entity.EntityType;
            Description = entity.Description;
            DateAdded = entity.DateAdded;
            EntityStatus = EntityStatusEnum.Active;
            IconUrl = entity.IconUrl;
            AdditionalData = additionalData;
        }

        public IndexedEntityModel(string text, long id, string description, string iconUrl, EntityTypeEnum type, string additionalData = null)
        {
            Name = text;
            Id = id;
            Description = description;
            IconUrl = iconUrl;
            EntityType = type;
            AdditionalData = additionalData;
        }

        public ChoiceItemModel<long> ToChoiceItemModel()
        {
            return new ChoiceItemModel<long>
            {
                Text = Name,
                Value = Id,
                IconUrl = IconUrl,
                Type = EntityType,
                AdditionalData = AdditionalData
            };
        }
    }
}
