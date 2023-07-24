using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    public class StageBracketModel : BaseEntityModel
    {
        [ForeignKey(nameof(Stage))]
        public long StageId { get; set; }
        public string Name { get; set; }
        [ForeignKey(nameof(AddedBy))]
        public long AddedById { get; set; }
        public AccountModel AddedBy { get; set; }
        [JsonIgnore]
        public TournamentStageModel Stage { get; set; }
    }
}
