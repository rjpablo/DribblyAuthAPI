namespace Dribbly.Core.Models
{
    public class PagedGetInputModel
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
