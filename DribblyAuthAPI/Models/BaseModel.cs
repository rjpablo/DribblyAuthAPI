using System;
using System.ComponentModel.DataAnnotations;

namespace DribblyAuthAPI.Models
{
    public abstract class BaseModel
    {
        public DateTime DateAdded { get; set; }
        [Key]
        public long Id { get; set; }
        public BaseModel()
        {            
        }
    }
}