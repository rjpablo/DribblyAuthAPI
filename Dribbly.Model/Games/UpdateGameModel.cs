using Dribbly.Service.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Games
{
    [NotMapped]
    public class UpdateGameModel:GameModel
    {
        public GameStatusEnum? ToStatus { get; set; }
    }
}
