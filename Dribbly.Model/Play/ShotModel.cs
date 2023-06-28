using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Courts;
using Dribbly.Model.Entities;
using Dribbly.Model.Shared;
using Dribbly.Model.Teams;
using Dribbly.Service.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Games
{
    [Table("Shots")]
    public class ShotModel : GameEventModel
    {
        #region MappedColumns
        public int Points { get; set; }
        public bool IsMiss { get; set; }
        public long? TakenById { get; set; }
        #endregion

    }
}