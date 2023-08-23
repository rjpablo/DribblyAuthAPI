using Dribbly.Core.Enums;

namespace Dribbly.Core.Models
{
    public class MultimediaModel : BaseEntityModel
    {
        public MultimediaModel() { }
        public MultimediaModel(string url, MultimediaTypeEnum type, string title = "", string description = "")
        {
            Url = url;
            Type = type;
            Title = title;
            Description = description;
        }
        public string Url { get; set; }
        public MultimediaTypeEnum Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
