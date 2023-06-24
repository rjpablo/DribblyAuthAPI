using Dribbly.Model.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Fouls
{
    [Table("Fouls")]
    public class FoulModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public FoulTypeEnum Type { get; set; }
        /// <summary>
        /// Whether or not this can be called on an offensive player
        /// </summary>
        public bool IsOffensive { get; set; }
        /// <summary>
        /// Whether or not this can be called on an deffensive player
        /// </summary>
        public bool IsDefensive { get; set; }
        public bool IsTechnical { get; set; }
        [NotMapped]
        public bool IsFlagrant { get => Name.Contains("Flagrant"); }

        public FoulModel() { }

        public FoulModel(int id, string name, FoulTypeEnum type, bool isOffensive, bool isDefensive, bool isTechnical = false)
        {
            Id = id;
            Name = name;
            Type = type;
            IsOffensive = isOffensive;
            IsDefensive = isDefensive;
            IsTechnical = isTechnical;
        }
    }
}
