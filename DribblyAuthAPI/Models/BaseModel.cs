using System;

namespace DribblyAuthAPI.Models
{
    public abstract class BaseModel
    {
        public DateTime DateAdded { get; set; }
        public BaseModel()
        {            
        }
    }
}