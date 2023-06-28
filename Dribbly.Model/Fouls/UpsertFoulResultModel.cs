using Dribbly.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.Fouls
{
    public class UpsertFoulResultModel
    {
        /// <summary>
        /// The player's total number of personal fouls
        /// </summary>
        public int TotalPersonalFouls { get; set; }
        /// <summary>
        /// The player's total number of technical fouls
        /// </summary>
        public int TotalTechnicalFouls { get; set; }
        public bool IsEjected { get; set; }
        public bool HasFouledOut { get; set; }
        public EjectionStatusEnum EjectionStatus { get; set; }
    }
}
