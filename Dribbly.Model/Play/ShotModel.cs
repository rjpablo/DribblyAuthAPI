﻿using Dribbly.Model.Entities;
using Dribbly.Model.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Games
{
    [Table("Shots")]
    public class ShotModel : GameEventModel
    {
        #region MappedColumns
        public int Points { get; set; }
        public bool IsMiss { get; set; }
        public ShotTypeEnum ShotType { get; set; }
        #endregion

    }
}