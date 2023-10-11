namespace Dribbly.Model.Account
{
    public class AccountSearchInputModel
    {
        public string NameSegment { get; set; }
        public long[] ExcludeIds { get; set; }
    }
}
