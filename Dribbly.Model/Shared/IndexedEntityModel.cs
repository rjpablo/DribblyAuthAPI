using Dribbly.Service.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Shared
{
    [Table("IndexedEntities")]
    public class IndexedEntityModel
    {
        [Key, Column(Order = 1)]
        public long Id { get; set; }

        [Key, Column(Order = 2)]
        public EntityTypeEnum Type { get; set; }
        public string Name { get; set; }

        public string IconUrl { get; set; }

        public EntityStatusEnum Status { get; set; }

        public string Description { get; set; }

        public DateTime DateAdded { get; set; }

        public IndexedEntityModel() { }

        public IndexedEntityModel(string text, long id, string description, string iconUrl, EntityTypeEnum type)
        {
            Name = text;
            Id = id;
            Description = description;
            IconUrl = iconUrl;
            Type = type;
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
