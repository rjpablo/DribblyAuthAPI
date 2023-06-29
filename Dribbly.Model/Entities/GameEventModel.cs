﻿using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Enums;
using Dribbly.Model.Games;
using Dribbly.Model.Teams;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    [Table("GameEvents")]
    public class GameEventModel: BaseEntityModel
    {
        [ForeignKey(nameof(Game))]
        public long GameId { get; set; }
        [ForeignKey(nameof(Team))]
        public long TeamId { get; set; }
        [ForeignKey(nameof(PerformedBy))]
        public long PerformedById { get; set; }
        public GameEventTypeEnum Type { get; set; }
        public string AdditionalData { get; set; }
        public int? Period { get; set; }
        /// <summary>
        /// The time on the game clock when the event happened
        /// </summary>
        public int? ClockTime { get; set; }
        /// <summary>
        /// The id of the shot linked to this event, if any
        /// </summary>
        [ForeignKey(nameof(Shot))]
        public long? ShotId { get; set; }
        public TeamModel Team { get; set; }
        [JsonIgnore]
        public GameModel Game { get; set; }
        public AccountModel PerformedBy { get; set; }
        /// <summary>
        /// The shot linked to this event, if any
        /// </summary>
        public GameEventModel Shot { get; set; }
    }
}
