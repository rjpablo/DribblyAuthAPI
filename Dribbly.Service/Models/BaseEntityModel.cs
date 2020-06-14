using System;
using System.ComponentModel.DataAnnotations;

namespace Dribbly.Service.Models
{
    /// <summary>
    /// Models that are mapped to a database table should extend this model
    /// to enable usage of common functions in BaseEntiryService.
    /// Models that NOT have Id, and DateAdded fields should extend BaseModel
    /// intead.
    /// </summary>
    public abstract class BaseEntityModel : BaseModel
    {
        public DateTime DateAdded { get; set; }
        [Key]
        public long Id { get; set; }
        public BaseEntityModel()
        {
        }
    }
}