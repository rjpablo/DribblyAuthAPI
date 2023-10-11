using Dribbly.Core.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Shared
{
    [Table("Contacts")]
    public class ContactModel : BaseEntityModel
    {
        public string Number { get; set; }
        public long AddedBy { get; set; }
    }
}
