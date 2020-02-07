using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DribblyAuthAPI.Models.Courts
{
    [Table("Settings")]
    public class SettingModel : BaseModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string DefaultValue { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public new DateTime? DateAdded { get; set; }

        public SettingModel() { }

        public SettingModel(long id, string key, string description, string defaultValue, string value = "")
        {
            Id = id;
            Key = key;
            Value = value;
            Description = description;
            DefaultValue = defaultValue;
        }
    }
}