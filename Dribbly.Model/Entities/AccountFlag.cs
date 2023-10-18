using Dribbly.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dribbly.Model.Entities
{
    [Table("AccountFlags")]
    public class AccountFlag
    {
        [Key, Column(Order = 1)]
        public string Key { get; set; }
        [Key, Column(Order = 2)]
        public long AccountId { get; set; }
        public string Value { get; set; }
        public AccountModel Account { get; set; }
        public AccountFlag() { }
        public AccountFlag(long accountId, string key, string value = null)
        {
            AccountId = accountId;
            Key = key;
            Value = value;
        }
    }
}
