using System.ComponentModel.DataAnnotations;

namespace Dribbly.Core.Models
{
    public abstract class PlaceModel
    {
        [Key]
        public long Id { get; set; }
        public string GoogleId { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Latitude
        /// </summary>
        public double Lat { get; set; }
        /// <summary>
        /// Longitude
        /// </summary>
        public double Lng { get; set; }
    }
}
