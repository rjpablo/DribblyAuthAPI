using Dribbly.Core.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Courts;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Games
{
    // Games table has an additional ID column which is a Foreign Key to the bookings table
    [Table("Games")]
    public class GameModel : BaseEntityModel, IIndexedEntity
    {
        #region MappedColumns
        [Required]
        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }

        [MinLength(5)]
        public string Title { get; set; }

        [Required]
        public long AddedById { get; set; }

        [ForeignKey("Court"), Required]
        public long CourtId { get; set; }

        public GameStatusEnum Status { get; set; }

        public long? Team1Id { get; set; }

        public long? Team2Id { get; set; }

        public long? WinningTeamId { get; set; }

        [NotMapped]
        public EntityTypeEnum EntityType { get; } = EntityTypeEnum.Game;

        #endregion

        [NotMapped]
        public string Name { get { return Title; } }
        public EntityStatusEnum EntityStatus { get; set; }
        [NotMapped]
        public string IconUrl { get { return ""; } }
        [NotMapped]
        public string Description { get { return ""; } }

        // navigation properties
        public virtual CourtModel Court { get; set; }

        public virtual AccountBasicInfoModel AddedBy { get; set; }
    }
}