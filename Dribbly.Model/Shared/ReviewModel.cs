using Dribbly.Core.Models;
using Dribbly.Model.Account;

namespace Dribbly.Model.Shared
{
    public abstract class ReviewModel : BaseEntityModel
    {
        public double Rating { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Maps to <see cref="ApplicationUser"/>
        /// </summary>
        public long ReviewedById { get; set; }

        public virtual AccountBasicInfoModel ReviewedBy { get; set; }
    }
}
