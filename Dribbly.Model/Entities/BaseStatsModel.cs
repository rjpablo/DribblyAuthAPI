﻿using Dribbly.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.Entities
{
    public class BaseStatsModel : BaseEntityModel
    {
        public int Points { get; set; }
        /// <summary>
        /// field goal attempts
        /// </summary>
        public int FGA { get; set; }
        /// <summary>
        /// field goals made
        /// </summary>
        public int FGM { get; set; }
        /// <summary>
        /// free throw attempts
        /// </summary>
        public int FTA { get; set; }
        /// <summary>
        /// free throws made
        /// </summary>
        public int FTM { get; set; }
        /// <summary>
        /// 3pt attempts
        /// </summary>
        public int ThreePA { get; set; }
        /// <summary>
        /// 3pts made
        /// </summary>
        public int ThreePM { get; set; }
        public int Blocks { get; set; }
        public int Rebounds { get; set; }
        public int Assists { get; set; }
        public int Turnovers { get; set; }
        public int Steals { get; set; }
        public bool? Won { get; set; }
    }

    public interface IBaseStatsModel
    {
        int Points { get; set; }
        /// <summary>
        /// field goal attempts
        /// </summary>
        int FGA { get; set; }
        /// <summary>
        /// field goals made
        /// </summary>
        int FGM { get; set; }
        /// <summary>
        /// free throw attempts
        /// </summary>
        int FTA { get; set; }
        /// <summary>
        /// free throws made
        /// </summary>
        int FTM { get; set; }
        /// <summary>
        /// 3pt attempts
        /// </summary>
        int ThreePA { get; set; }
        /// <summary>
        /// 3pts made
        /// </summary>
        int ThreePM { get; set; }
        int Blocks { get; set; }
        int Rebounds { get; set; }
        int Assists { get; set; }
        int Turnovers { get; set; }
        int Steals { get; set; }
        bool? Won { get; set; }
    }
}
