using Dribbly.Core.Enums;

namespace Dribbly.Core.Models
{
    public class ChoiceItemModel<TValue>
    {
        public string Text { get; set; }

        public TValue Value { get; set; }

        public string IconUrl { get; set; }

        public EntityTypeEnum  Type { get; set; }

        public string AdditionalData { get; set; }

        public ChoiceItemModel() { }

        public ChoiceItemModel(string text, TValue value, string iconUrl, EntityTypeEnum type, string additionalData = null)
        {
            Text = text;
            Value = value;
            IconUrl = iconUrl;
            Type = type;
            AdditionalData = additionalData;
        }
    }
}
