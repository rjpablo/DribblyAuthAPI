using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Courts;
using Dribbly.Model.Shared;
using Dribbly.Model.Teams;
using Dribbly.Service.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Games
{
    // Games table has an additional ID column which is a Foreign Key to the bookings table
    [Table("Shots")]
    public class ShotModel : BaseEntityModel
    {
        #region MappedColumns
        public int Points { get; set; }
        public bool IsMiss { get; set; }
        public long? TakenById { get; set; }
        /// <summary>
        /// The ID of the team to which the player who took the shot belongs to
        /// </summary>
        [ForeignKey(nameof(Team))]
        public long TeamId { get; set; }
        /// <summary>
        /// The ID of the game being played
        /// </summary>
        [ForeignKey(nameof(Game))]
        public long GameId { get; set; }
        #endregion

        public GameModel Game { get; set; }
        public TeamModel Team { get; set; }

    }
}