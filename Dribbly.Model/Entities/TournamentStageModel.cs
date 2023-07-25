using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Enums;
using Dribbly.Model.Games;
using Dribbly.Model.Tournaments;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    [Table("TournamentStages")]
    public class TournamentStageModel : BaseEntityModel
    {

        [ForeignKey(nameof(Tournament))]
        public long TournamentId { get; set; }

        [ForeignKey(nameof(AddedBy))]
        public long AddedById { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public StageStatusEnum Status { get; set; }

        #region Navigational Properties
        [JsonIgnore]
        public TournamentModel Tournament { get; set; }
        public AccountModel AddedBy { get; set; }
        #endregion

        public ICollection<StageBracketModel> Brackets { get; set; } = new List<StageBracketModel>();
        public ICollection<StageTeamModel> Teams { get; set; } = new List<StageTeamModel>();
        public ICollection<GameModel> Games { get; set; } = new List<GameModel>();
    }
}
